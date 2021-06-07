using Godot;
using System;

using ReplicationAbstractions;

public class InventoryMenu : Spatial
{

    public static NodeFactory<InventoryMenu> Factory
        = new NodeFactory<InventoryMenu>("res://BasicScenes/Items/InventoryMenu.tscn");
    
    IHasLootPV root = null;
    Spatial rootObserver;
    Camera cam;
    RayCast mouseRay;
    Spatial rootHandle;
    Spatial RayEndpoint;

    //Need to keep track of this for when we leave the mouseover.
    PickableArea currentMouseOver = null;
    //Initing these to zero is fine
    //because 
    Vector3 clickOnPos = new Vector3();
    uint clickOnTime = 0;
    PickableArea clickOnNode = null;

    [Signal]
    public delegate void RayUpdated(Transform ray);

    public void Subscribe(IHasLootPV _root)
    {
        root = _root;
        rootObserver = (Spatial) EasyInstancer.GenObserver( (Node) root, root.ObserverPathLootPV);
        
        //This is needed before we enter the tree.
        //So we do it here instead of _Ready()
        mouseRay = GetNode<RayCast>("MouseRay");
        RayEndpoint = mouseRay.GetNode<Spatial>("Endpoint");
        rootHandle = GetNode<Spatial>("RootHandle");
        rootHandle.AddChild(rootObserver);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        var mouseMoveEvent = inputEvent as InputEventMouseMotion;
        if(!(mouseMoveEvent is null))
        {
            //Check the previous raycast
            //(since we can't do physics in input functions)
            //but that's fine since this function gets called multiple times
            //for any single mouse movement.
            PollRayCast();

            var to = cam.ProjectLocalRayNormal(mouseMoveEvent.Position);
            
            //scale so that it's always ending exactly on the z plane of the inventory.
            mouseRay.CastTo = to  * rootHandle.Translation.z / to.z;
            RayEndpoint.Translation = mouseRay.CastTo;
            EmitSignal(nameof(RayUpdated),RayEndpoint.GlobalTransform);
            
            GetTree().SetInputAsHandled();
        }
        else if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            if(!(currentMouseOver is null))
            {
                clickOnTime = OS.GetTicksMsec();
                //we want pos to be scaled to z=1
                clickOnPos = mouseRay.CastTo /mouseRay.CastTo.z;
                clickOnNode = currentMouseOver;
                currentMouseOver = null;
                clickOnNode.Press(this);
            }
            GetTree().SetInputAsHandled();
        }
        else if (inputEvent.IsActionReleased("ItemPrimary") && !(clickOnNode is null))
        {
            
            //check if its just a regular click.
            var t = OS.GetTicksMsec() - clickOnTime;
            var distance = (mouseRay.CastTo/mouseRay.CastTo.z).DistanceTo(clickOnPos);
            
            //All of this is just for ILootPV stuff, so we cast it beforehand.

            var lootItem = clickOnNode as DefaultLootPV;

            if(!(lootItem is null))
            {// The thing being released is an ILootPV
             // It has special behavior.
                if(t < 100 && distance<0.1)
                {
                    //it was a regular click.
                    //Might want to center UI on item when we do this
                    //Not necessary yet though.
                    lootItem.FullClick();
                }
                else if(currentMouseOver is IAcceptsDrop recipient)
                {
                    //This was a click and drag
                    //and we may be dropping it on something.
                    //And that thing can take a drop.

                    recipient.Drop(lootItem);
                }
            }
            
            //Release regardless
            clickOnNode.Release(this);
            clickOnNode = null;
            GetTree().SetInputAsHandled();
        }
    }

    private void PollRayCast()
    {
        PickableArea pickedSlot = mouseRay.GetCollider() as PickableArea;

        //The use of "null" as "not colliding with anything makes
        //null conditional operators very convenient.
        if(pickedSlot != currentMouseOver)
        {
            currentMouseOver?.MouseOff();
            currentMouseOver = pickedSlot;
            currentMouseOver?.MouseOn();
        }
    }

    public override void _EnterTree()
    {
        if(cam is null)
            cam = (Camera) GetParent();
        Input.SetMouseMode(Input.MouseMode.Visible);
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

}
