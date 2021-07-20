using Godot;
using System;
using System.Collections.Generic;

public class DraggableArea : Area, IPickable
{

    public bool Permeable {get;set;} = false;
    ClickDragTracker M1 = new ClickDragTracker("MousePrimary");
    
    public virtual void MouseOn(TwoFiveDMenu _menu)
    {
        GD.Print(Name, ": Moused on");
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff(TwoFiveDMenu _menu)
    {
        GD.Print(Name, ": Moused off");
    }

    public virtual void OnRayUpdated( RayCast ray)
    {
        
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        M1._UnhandledInput(inputEvent);
    }
}

public class ClickDragTracker : Node
{
    readonly string actionName;

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

    Vector3 cursorPos;
    Vector3 clickedPos;

    public ClickDragTracker(string _actionName)
    {
        actionName = _actionName;
    }

    public void UpdateCursor( RayCast ray)
    {
        cursorPos = ray.CastTo/ray.CastTo.z; //normalize to z=1
        if( clickState == ClickState.Down && cursorPos.DistanceTo(clickedPos) > 0.1)
            clickState = ClickState.Dragging;
    }

    //similar signature and naming convention to node
    //if we want to turn this into a node at some point.
    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed(actionName))
        {
            //started click or click and drag. Don't know which yeet.
            clickState = ClickState.Down;
            
            clickedPos = cursorPos;
        }
        else if(inputEvent.IsActionReleased(actionName) && clickState != ClickState.Up)
        {
            if(clickState == ClickState.Down)
                clickState = ClickState.Dragging;

            menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(UpdateCursor));
            
            //Remove us, or if we're still moused over, move us to mouseOver
            InputPriorityServer.Unsubscribe(this);
            if(menu.currentMouseOver == this)
                InputPriorityServer.Subscribe(InputPriorityServer.mouseOver, this);
        }
    }
    
}