using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public interface IAcceptsDrop
{
    void Drop( DefaultLootPV item);
}

public class TwoFiveDMenu : RayCast, ITakesInput
{

    public static NodeFactory<TwoFiveDMenu> Factory
        = new NodeFactory<TwoFiveDMenu>("res://BasicScenes/GUI/2.5D UI/TwoFiveDMenu.tscn");
    
    public InputClaims Claims {get;set;} = new InputClaims();

    Camera cam;

    //Need to keep track of this for when we leave the mouseover.
    List<Node> lastMouseOvers = new List<Node>();

    const int pickingLimit = 10;
    
    [Signal]
    public delegate void MouseUpdated(TwoFiveDMenu ray);

    public override void _Ready()
    {
        cam = (Camera) GetParent();
        
        //Technically we also use mouse motion,
        //but that isn't accessible through the Input singleton
        //So we don't claim it doesn't really exist.
        Claims.Claims.UnionWith(InputPriorityServer.Instance.mouseButtons);
    }

    //Primarily if stuff moves around
    //even though the mouse isn't moving.
    //(such as pickable Areas or the camera itself.)
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
        //Honestly this is overkill considering you regularly only have 2 lol.
        GD.Print(mouseOvers.Count);
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
        
        //Clear Exceptions so we start next frame on a clean slate.
        lastMouseOvers = mouseOvers;
        ClearExceptions();
    }

    public bool OnInput(InputEvent inputEvent)
    {
        
        var mouseMoveEvent = inputEvent as InputEventMouseMotion;
        if(!(mouseMoveEvent is null))
        {
            
            //scale to z=1e6 so it's huge. Don't want to miss anything.
            CastTo = cam.ProjectLocalRayNormal(mouseMoveEvent.Position) * 1.0e3f;
            EmitSignal(nameof(MouseUpdated), this);
            return true;
        }
        // else if (inputEvent.IsActionReleased("MousePrimary") && !(clickOnNode is null))
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
        // else if (inputEvent.IsActionPressed("MouseSecondary"))
        //     rootHandle.Attach(this);
        // else if (inputEvent.IsActionReleased("MouseSecondary"))
        //     rootHandle.Detach();
        return false;
    }

    public override void _EnterTree()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        InputPriorityServer.Base.Subscribe(this, BaseRouter.menu);
    }

    public override void _ExitTree()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
        InputPriorityServer.Base.Unsubscribe(this, BaseRouter.menu);
    }

}
