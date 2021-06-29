using Godot;
using System;

using ReplicationAbstractions;

public interface IAcceptsDrop
{
    void Drop( DefaultLootPV item);
}

public class TwoFiveDMenu : Spatial
{

    public static NodeFactory<TwoFiveDMenu> Factory
        = new NodeFactory<TwoFiveDMenu>("res://BasicScenes/Items/TwoFiveDMenu.tscn");
    
    Camera cam;
    RayCast mouseRay;
    Spatial rootHandle;

    //Need to keep track of this for when we leave the mouseover.
    Pickable currentMouseOver = null;
    //Initing these to zero is fine
    Vector3 clickOnPos = new Vector3();
    uint clickOnTime = 0;
    Pickable clickOnNode = null;

    [Signal]
    public delegate void RayUpdated(RayCast ray);

    public override void _Ready()
    {
        cam = (Camera) GetParent();
        mouseRay = GetNode<RayCast>("MouseRay");
        rootHandle = GetNode<Spatial>("RootHandle");
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        var mouseMoveEvent = inputEvent as InputEventMouseMotion;
        if(!(mouseMoveEvent is null))
        {

            var to = cam.ProjectLocalRayNormal(mouseMoveEvent.Position);
            
            //scale to z=1e6 so it's huge. Don't want to miss anything.
            mouseRay.CastTo = to  * rootHandle.Translation.z / to.z * 1.0e6f;
            EmitSignal(nameof(RayUpdated), mouseRay);

            //Update Raycast after signal so picked items can react to mouse movement.
            mouseRay.ForceRaycastUpdate();
            Pickable pickedObject = mouseRay.GetCollider() as Pickable;

            //The use of "null" as "not colliding with anything makes
            //null conditional operators very convenient.
            if(pickedObject != currentMouseOver)
            {
                currentMouseOver?.MouseOff(this);
                currentMouseOver = pickedObject;
                currentMouseOver?.MouseOn(this);
            }

            //This is the highest up the tree a mouse move event should go
            //if this node is in the tree.
            GetTree().SetInputAsHandled();
        }
        // else if(inputEvent.IsActionPressed("ItemPrimary"))
        // {
        //     clickOnPos = mouseRay.CastTo /mouseRay.CastTo.z;
                
        //     if(!(currentMouseOver is null))
        //     {
        //         //we want pos to be scaled to z=1
        //         clickOnNode = currentMouseOver;
        //         currentMouseOver = null;
        //         clickOnNode.Press(this);
        //     }
        //     GetTree().SetInputAsHandled();
        // }
        // else if (inputEvent.IsActionReleased("ItemPrimary") && !(clickOnNode is null))
        // {
            
        //     var distance = (mouseRay.CastTo/mouseRay.CastTo.z).DistanceTo(clickOnPos);
            
        //     //All of this is just for ILootPV stuff, so we cast it beforehand.

        //     var lootItem = clickOnNode as DefaultLootPV;

        //     if(!(lootItem is null))
        //     {// The thing being released is a DefaultLootItem
        //      // It has special behavior.
               
        //         //it was a regular click.
        //         //Might want to center UI on item when we do this
        //         //Not necessary yet though.
        //         lootItem.FullClick();
                
        //         // else
        //         // {
        //         //     if(currentMouseOver is IAcceptsDrop recipient)
        //         //         recipient.Drop(lootItem);
        //         //     lootItem.parent.RecomputeOccupantPos();
        //         // }       
        //     }
        //     //Release regardless
        //     clickOnNode.Release(this);
        //     clickOnNode = null;
        //     GetTree().SetInputAsHandled();
        // }
        //This stuff will need to be genericized.
        // else if (inputEvent.IsActionPressed("ItemSecondary"))
        //     rootHandle.Attach(this);
        // else if (inputEvent.IsActionReleased("ItemSecondary"))
        //     rootHandle.Detach();
        
    }

    public override void _EnterTree()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void _ExitTree()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

}
