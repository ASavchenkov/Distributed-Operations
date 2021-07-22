using Godot;
using System;
using System.Collections.Generic;

public class DraggableArea : Area, IPickable
{

    public bool Permeable {get;set;} = false;
    protected ClickDragTracker M1 = new ClickDragTracker("MousePrimary");

    
    public virtual void MouseOn(TwoFiveDMenu _menu)
    {
        GD.Print(Name, ": Moused on");
        M1.menu = _menu;
        InputPriorityServer.BaseRouter.Subscribe(M1, InputPriorityServer.mouseOver);
        
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff(TwoFiveDMenu _menu)
    {
        GD.Print(Name, ": Moused off");
        InputPriorityServer.BaseRouter.Subscribe(M1, InputPriorityServer.mouseOver);
    }
}

public class ClickDragTracker : Godot.Object, ITakesInput
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
    [Signal]
    public delegate void StateUpdate(ClickState state);
    [Signal]
    public delegate void FullClick();
    [Signal]
    public delegate void Drag();
    [Signal]
    public delegate void Drop();

    Vector3 cursorPos;
    Vector3 clickedPos;

    public ClickDragTracker(string _actionName)
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
            InputPriorityServer.BaseRouter.Subscribe(this, InputPriorityServer.dragging);
            return true;
        }
        else if(inputEvent.IsActionReleased(actionName))
        {
                
            if(clickState == ClickState.Down)
                EmitSignal(nameof(FullClick));
            else //end of drag. Dropping.
                EmitSignal(nameof(Drop));

            InputPriorityServer.BaseRouter.Unsubscribe(this, InputPriorityServer.dragging);
            menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(UpdateCursor));
            clickState = ClickState.Up;
            return true;
        }
        return false;
    }
    
}