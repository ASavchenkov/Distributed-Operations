using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using ReplicationAbstractions;

public class TwoFiveDMenu : RayCast, ITakesInput
{

    public static NodeFactory<TwoFiveDMenu> Factory
        = new NodeFactory<TwoFiveDMenu>("res://BasicScenes/GUI/2.5D UI/TwoFiveDMenu.tscn");
    
    public InputClaims Claims {get;set;} = new InputClaims();

    // [Export]
    // public string camPath;
    public string camPath = "..";
    Camera cam;

    public List<Spatial> mouseIntersections {get; private set;} = new List<Spatial>();
    public Dictionary<Spatial, Vector3> intersectionPoints {get; private set;} = new Dictionary<Spatial, Vector3>();
    
    private OrderedRouter mouseOverRouter = new OrderedRouter();
    
    const int pickingLimit = 10;

    [Signal]
    public delegate void MouseUpdated();

    public override void _Ready()
    {
        cam = (Camera) GetNode(camPath);
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
        var newIntersections = new List<Spatial>(); 
        var newMouseOvers = new List<ITakesInput>();
        bool hitNonPermeable = false;
        //Populate mouseOvers
        for(int i = 0; i< pickingLimit; i++)
        {
            //IDK how this could not be a spatial, since we're in 3D exclusively,
            //but if it is then god help us all.
            var collidedObject = GetCollider() as Spatial;
            if(collidedObject is null) break;

            newIntersections.Add(collidedObject);
            intersectionPoints[collidedObject] = GetCollisionPoint();

            if(!hitNonPermeable && collidedObject is IPickable p)
            {
                if(!p.Permeable) hitNonPermeable = true;
                newMouseOvers.Add(p);
            }

            AddException(collidedObject);
            ForceRaycastUpdate();
        }

        //Call mouseOn/mouseOff to any changed IPickables.
        //Honestly this is overkill considering you regularly only have 2 lol.
        foreach(ITakesInput t in newMouseOvers)
        {
            if(!mouseOverRouter.LayerPriorities.Contains(t))
                ((IPickable) t).MouseOn(this);
        }
        foreach(ITakesInput t in mouseOverRouter.LayerPriorities)
        {
            if(!newMouseOvers.Contains(t))
            {
                ((IPickable) t).MouseOff();
            }
        }
        mouseOverRouter.LayerPriorities = newMouseOvers;

        List<Spatial> removals = (from ip in intersectionPoints.Keys
                                where !newIntersections.Contains(ip)
                                select ip).ToList<Spatial>();
        foreach(Spatial s in removals)
        {
            intersectionPoints.Remove(s);
        }
        
        mouseIntersections = newIntersections;
        //Clear Exceptions so we start next frame on a clean slate.
        ClearExceptions();
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent is InputEventMouseMotion mouseMoveEvent)
        {
            //scale so it's huge. Don't want to miss anything.
            Translation = cam.ToLocal(cam.ProjectRayOrigin(mouseMoveEvent.Position));
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
        InputPriorityServer.Base.Subscribe(mouseOverRouter, BaseRouter.mouseOver);
    }

    public override void _ExitTree()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
        InputPriorityServer.Base.Unsubscribe(this, BaseRouter.menu);
        InputPriorityServer.Base.Unsubscribe(mouseOverRouter, BaseRouter.mouseOver);
    }

}
