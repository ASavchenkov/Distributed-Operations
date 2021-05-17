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

    //Need to keep track of this for when we leave the mouseover.
    ILootPickable currentMouseOver = null;

    //Initing these to zero is fine
    //because 
    Vector3 clickOnPos = new Vector3();
    uint clickOnTime = 0;

    [Signal]
    public delegate void RayUpdated(Vector3 castTo);    

    public void Subscribe(IHasLootPV _root)
    {
        root = _root;
        rootObserver = (Spatial) EasyInstancer.GenObserver( (Node) root, root.ObserverPathLootPV);
        
        //These should stay valid as it gets moved in and out of the tree.
        //So we do it here instead of _Ready()
        mouseRay = (RayCast) GetNode("MouseRay");
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
            GD.Print(mouseRay.CastTo);
            EmitSignal(nameof(RayUpdated), mouseRay.CastTo);

            GetTree().SetInputAsHandled();
        }
        else if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            if(!(currentMouseOver is null))
            {
                clickOnTime = OS.GetTicksMsec();
                clickOnPos = mouseRay.CastTo;
                currentMouseOver.pMember.Press(this);
            }
            GetTree().SetInputAsHandled();
        }
        else if (inputEvent.IsActionReleased("ItemPrimary"))
        {

        }
    }

    private void PollRayCast()
    {
        ILootPickable pickedSlot = mouseRay.GetCollider() as ILootPickable;

        //The use of "null" as "not colliding with anything makes
        //null conditional operators very convenient.
        if(pickedSlot != currentMouseOver)
        {
            currentMouseOver?.pMember?.MouseOff();
            currentMouseOver = pickedSlot;
            currentMouseOver?.pMember?.MouseOn();
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
