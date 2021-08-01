using Godot;
using System;
using System.Collections.Generic;

//Hardcoded Mouse Button 1 dragging.
//Will figure out better abstraction for different
//button choices and dragging behavior later.
//(when it becomes relevant)
public abstract class DraggableArea : Area, IPickable
{

    public bool Permeable {get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    protected MouseActionTracker M1 = new MouseActionTracker("MousePrimary");

    protected TwoFiveDMenu menu = null;
    
    public override void _Ready()
    {
        Claims = M1.Claims;// Just link to M1 for now since it's the only one.
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrop));
    }

    public virtual void MouseOn(TwoFiveDMenu _menu)
    {
        GD.Print(Name, ": Moused on");
        menu = _menu;
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff()
    {
        GD.Print(Name, ": Moused off");
    }

    public virtual void OnDrag()
    {
        menu.Connect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
        M1.Connect(nameof(MouseActionTracker.Drop), this, nameof(OnDrop));
        Permeable = true;
    }

    public virtual void OnDrop()
    {
        menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
        M1.Disconnect(nameof(MouseActionTracker.Drop), this, nameof(OnDrop));
        Permeable = false;
    }

    public abstract void OnMouseUpdate();

    public bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }
}

public class MouseActionTracker : Godot.Object, ITakesInput
{

    public InputClaims Claims {get;set;} = new InputClaims();

    readonly string actionName;
    public TwoFiveDMenu menu;
    
    public enum ClickState { Up, Down, Dragging};
    ClickState _clickState = ClickState.Up;
    public ClickState clickState
    {
        get => _clickState;
        set
        {
            _clickState = value;
            EmitSignal(nameof(StateUpdate), _clickState);
        }
    }

    //Signals for common mouse actions.
    [Signal]
    public delegate void FullClick();
    [Signal]
    public delegate void Drag();
    [Signal]
    public delegate void Drop();
    
    //Raw state update if none of the standard signals fit.
    [Signal]
    public delegate void StateUpdate(ClickState state);
 
    Vector3 cursorPos;
    Vector3 clickedPos;

    public MouseActionTracker(string _actionName)
    {
        actionName = _actionName;
        Claims.Claims.Add(actionName);
    }

    public void UpdateCursor( RayCast ray)
    {
        cursorPos = ray.CastTo/ray.CastTo.z; //normalize to z=1
        if( clickState == ClickState.Down && cursorPos.DistanceTo(clickedPos) > 0.1)
        {
            clickState = ClickState.Dragging;
            EmitSignal(nameof(Drag));
        }
    }

    //similar signature and naming convention to node
    //if we want to turn this into a node at some point.
    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed(actionName))
        {
            //started click or click and drag. Don't know which yeet.
            clickState = ClickState.Down;
            clickedPos = cursorPos;
            InputPriorityServer.Base.Subscribe(this, BaseRouter.dragging);
            return true;
        }
        else if(inputEvent.IsActionReleased(actionName))
        {
                
            if(clickState == ClickState.Down)
                EmitSignal(nameof(FullClick));
            else //end of drag. Dropping.
                EmitSignal(nameof(Drop));

            InputPriorityServer.Base.Unsubscribe(this, BaseRouter.dragging);
            clickState = ClickState.Up;
            return true;
        }
        return false;
    }
    
}