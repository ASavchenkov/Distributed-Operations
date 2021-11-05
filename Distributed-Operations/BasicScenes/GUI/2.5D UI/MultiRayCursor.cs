using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

//Used for (potentially multiple overlapping) viewports with 3D cameras.
//Allows routing of inputs based on order of intersection by ray.
public class MultiRayCursor : Godot.Object, ITakesInput
{
    public InputClaims Claims {get;set;} = new InputClaims();
    private OrderedRouter mouseOverRouter = new OrderedRouter();

    List<PickingRay> rays = new List<PickingRay>();
    public int RayCount {get => rays.Count;}

    public List<Spatial> mouseIntersections {get; private set;} = new List<Spatial>();
    public Dictionary<Spatial, Vector3> intersectionPoints {get; private set;} = new Dictionary<Spatial, Vector3>();
    
    //Passes along actual mouse (or controller based simulated mouse) position
    public Vector2 MousePosition;
    [Signal]
    public delegate void CursorUpdated();

    bool _Enabled = false;
    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if(value != Enabled)
            {
                _Enabled = value;
                if(value)
                {
                    InputPriorityServer.Base.Subscribe(this,BaseRouter.menu);
                    InputPriorityServer.Base.Subscribe(mouseOverRouter, BaseRouter.mouseOver);
                    Input.SetMouseMode(Input.MouseMode.Visible);
                }
                else
                {
                    InputPriorityServer.Base.Unsubscribe(this,BaseRouter.menu);
                    InputPriorityServer.Base.Unsubscribe(mouseOverRouter, BaseRouter.mouseOver);
                    InputPriorityServer.RefreshMouseMode();
                }
            }
        }
    }

    public void InsertRay(int index, PickingRay ray)
    {
        rays.Insert(index, ray);
        Enabled = true;
    }
    public void RemoveRay(PickingRay ray)
    {
        rays.Remove(ray);
        if(rays.Count == 0)
            Enabled = false;
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent is InputEventMouseMotion mouseMoveEvent)
        {
            MousePosition = mouseMoveEvent.Position;
            EmitSignal(nameof(CursorUpdated));
            foreach( PickingRay ray in rays)
                ray.UpdateCursor(mouseMoveEvent.Position);
            return true;
        }
        if(inputEvent.IsAction("RefreshMouseMode") && Enabled)
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
            return true;
        }
        return false;
    }

    public void _PhysicsProcess(float delta)
    {
        const int pickingLimit = 10;
        var newIntersections = new List<Spatial>();
        var newMouseOvers = new List<ITakesInput>();
        bool hitNonPermeable = false;

        foreach(PickingRay ray in rays)
        {
            for(int i = 0; i< pickingLimit; i++)
            {
                //IDK how this could not be a spatial, since we're in 3D exclusively,
                //but if it isn't then god help us all.
                var collidedObject = ray.GetCollider() as Spatial;
                if(collidedObject is null) break;
                newIntersections.Add(collidedObject);
                intersectionPoints[collidedObject] = ray.GetCollisionPoint();

                if(!hitNonPermeable)
                {
                    if(collidedObject is LinkedArea la)
                    {
                        var p = (IPickable) la.GetNode(la.ParentPath);
                        newMouseOvers.Add(p);
                    }else if( collidedObject.GetParent() is IPickable p)
                    {
                        newMouseOvers.Add(p);
                    }
                }

                ray.AddException(collidedObject);
                ray.ForceRaycastUpdate();
            }
            ray.ClearExceptions();
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
    }
}
