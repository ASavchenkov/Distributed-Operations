using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public interface IAcceptsDrop
{
    void Drop( DefaultLootPV item);
}

public class TwoFiveDMenu : RayCast
{

    public static NodeFactory<TwoFiveDMenu> Factory
        = new NodeFactory<TwoFiveDMenu>("res://BasicScenes/Items/TwoFiveDMenu.tscn");
    
    Camera cam;
    Spatial rootHandle;

    //Need to keep track of this for when we leave the mouseover.
    public IPickable currentMouseOver = null;
    
    const int pickingLimit = 10;
    
    [Signal]
    public delegate void MouseUpdated(TwoFiveDMenu ray);

    public override void _Ready()
    {
        cam = (Camera) GetParent();
        rootHandle = GetNode<Spatial>("RootHandle");
    }

    //Primarily for stuff that might move around
    //even if the mouse isn't moving.
    //(such as pickable Areas or the camera itself.)
    //Also populates the list of intersected stuff.
    public override void _PhysicsProcess(float delta)
    {
        List<Node> mouseOvers = new List<Node>(); 

        //Populate mouseOvers
        for(int i = 0; i< pickingLimit; i++)
        {
            var collidedObject = GetCollider();
            
            if(collidedObject is null) break;
            if(collidedObject is IPickable p)
            {
                mouseOvers.Add((Node) p);
                if(!p.Permeable) break;

                AddException(collidedObject);
                ForceRaycastUpdate();
            }
            else    //should never be false (Only IPickable should be in picking layer)
            {
                GD.PrintErr("Non-IPickable node <", ((Node)collidedObject).Name, "> in picking layer");
                break;
            }
        }

        //Call mouseOn/mouseOff to any changed IPickables.
        var lastMouseOvers = (List<Node>) InputPriorityServer.layerNameMap[InputPriorityServer.mouseOver];
        mouseOvers.Reverse(); //So the "blocking" one is first in line.
        
        foreach(Node n in mouseOvers)
        {
            if(!lastMouseOvers.Contains(n))
                ((IPickable)n).MouseOn(this);
        }
        foreach(Node n in lastMouseOvers)
        {
            if(!mouseOvers.Contains(n))
                ((IPickable)n).MouseOff(this);
        }
        InputPriorityServer.SetLayer(InputPriorityServer.mouseOver, mouseOvers);
        
        //Clear Exceptions so we start next frame on a clean slate.
        ClearExceptions();
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        var mouseMoveEvent = inputEvent as InputEventMouseMotion;
        if(!(mouseMoveEvent is null))
        {

            var to = cam.ProjectLocalRayNormal(mouseMoveEvent.Position);
            
            //scale to z=1e6 so it's huge. Don't want to miss anything.
            CastTo = to  * rootHandle.Translation.z / to.z * 1.0e6f;
            EmitSignal(nameof(MouseUpdated), this);

            GetTree().SetInputAsHandled();
        }
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
