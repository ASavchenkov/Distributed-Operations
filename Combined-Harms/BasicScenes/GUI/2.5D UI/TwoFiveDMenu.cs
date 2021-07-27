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

    public List<Spatial> mouseOvers {get; private set;} = new List<Spatial>();
    public Dictionary<Spatial, Vector3> mouseIntersections {get; private set;} = new Dictionary<Spatial, Vector3>();

    const int pickingLimit = 10;

    [Signal]
    public delegate void MouseUpdated();

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
        List<Spatial> newMouseOvers = new List<Spatial>(); 

        //Populate mouseOvers
        for(int i = 0; i< pickingLimit; i++)
        {
            //IDK how this could not be a spatial, since we're in 3D exclusively,
            //but if it is then god help us all.
            var collidedObject = GetCollider() as Spatial;
            if(collidedObject is null) break;

            newMouseOvers.Add(collidedObject);
            mouseIntersections[collidedObject] = GetCollisionPoint();
            AddException(collidedObject);
            ForceRaycastUpdate();
        }

        //Call mouseOn/mouseOff to any changed IPickables.
        //Honestly this is overkill considering you regularly only have 2 lol.
        foreach(Spatial n in newMouseOvers)
        {
            if(!mouseOvers.Contains(n) && n is IPickable p)
                p.MouseOn(this);
        }
        foreach(Spatial n in mouseOvers)
        {
            if(!newMouseOvers.Contains(n))
            {
                mouseIntersections.Remove(n);
                if(n is IPickable p)
                    p.MouseOff();
            }
        }
        
        mouseOvers = newMouseOvers;
        //Clear Exceptions so we start next frame on a clean slate.
        ClearExceptions();
    }

    public bool OnInput(InputEvent inputEvent)
    {
        
        if(inputEvent is InputEventMouseMotion mouseMoveEvent)
        {
            //scale so it's huge. Don't want to miss anything.
            CastTo = cam.ProjectLocalRayNormal(mouseMoveEvent.Position) * 1e3f;
            EmitSignal(nameof(MouseUpdated));
            return true;
        }
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
