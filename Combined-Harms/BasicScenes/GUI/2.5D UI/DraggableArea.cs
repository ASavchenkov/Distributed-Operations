using Godot;
using System;

public class DraggableArea : Area, Pickable
{

    private enum ClickState { MouseUp, MouseDown, Dragging};
    private ClickState clickState = ClickState.MouseUp;
    public virtual void MouseOn(TwoFiveDMenu menu)
    {
        Connect(nameof(TwoFiveDMenu.RayUpdated), this, nameof(OnRayUpdated));
        GD.Print(Name, ": Moused on");
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff(TwoFiveDMenu menu)
    {
        Disconnect(nameof(TwoFiveDMenu.RayUpdated), this, nameof(OnRayUpdated));
        GD.Print(Name, ": Moused off");
    }

    public virtual void OnRayUpdated(RayCast ray)
    {
        
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            //started click or click and drag. Don't know which yeet.

        }
    }
}
