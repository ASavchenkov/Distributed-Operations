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

    public List<IPickable> mouseIntersections {get; private set;} = new List<IPickable>();
    public Dictionary<IPickable, Vector3> intersectionPoints {get; private set;} = new Dictionary<IPickable, Vector3>();
    
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
        var newIntersections = new List<IPickable>();
        var newMouseOvers = new List<ITakesInput>();
        bool hitNonPermeable = false;

        foreach(PickingRay ray in rays)
        {
            for(int i = 0; i< pickingLimit; i++)
            {
                //IDK how this could not be a spatial, since we're in 3D exclusively,
                //but if it isn't then god help us all.
                var collidedObject = ray.GetCollider() as IPickable;
                if(collidedObject is null) break;
                newIntersections.Add(collidedObject);
                intersectionPoints[collidedObject] = ray.GetCollisionPoint();

                if(!(collidedObject.PickingMember.InputRecipient is null))
                    newMouseOvers.Add(collidedObject.PickingMember.InputRecipient);

                if(!collidedObject.PickingMember.Permeable)
                {
                    hitNonPermeable = true;
                    break;
                }

                ray.AddException((Godot.Object) collidedObject);
                ray.ForceRaycastUpdate();
            }
            ray.ClearExceptions();
            if(hitNonPermeable)
                break;
        }

        //Call mouseOn/mouseOff to any changed IPickables.
        //Honestly this is overkill considering you regularly only have 2 lol.
        foreach(IPickable t in newIntersections)
        {
            if(!mouseIntersections.Contains(t))
                t.PickingMember.MouseOn(this);
        }
        foreach(IPickable t in mouseIntersections)
        {
            if(!newIntersections.Contains(t))
            {
                t.PickingMember.MouseOff();
                intersectionPoints.Remove(t);
            }
        }
        mouseOverRouter.LayerPriorities = newMouseOvers;
        mouseIntersections = newIntersections;
    }
}
