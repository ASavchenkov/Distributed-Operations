using Godot;
using System;

public class DefaultPickableArea : IPickable
{
    public PickingMixin PickingMember {get;set;}
}

public interface IPickable
{
    PickingMixin PickingMember {get;set;}
}

public class PickingMixin : Godot.Object
{
    //Not always present. null if not.
    public ITakesInput InputRecipient;
    public bool Permeable;

    public PickingMixin(ITakesInput recipient = null,  bool permeable = true, string mouseOnHandler = null, string mouseOffHandler = null)
    {
        InputRecipient = recipient;
        Permeable = permeable;
        if(!(InputRecipient is null))
        {
            if(!(mouseOnHandler is null))
                Connect(nameof(MousedOn), (Godot.Object) InputRecipient, mouseOnHandler);
            if(!(mouseOffHandler is null))
                Connect(nameof(MousedOff), (Godot.Object) InputRecipient, mouseOffHandler);
        }
    }

    [Signal]
    public delegate void MousedOn(MultiRayCursor cursor);
    [Signal]
    public delegate void MousedOff();

    public virtual void MouseOn(MultiRayCursor cursor)
    {
        EmitSignal(nameof(MousedOn),cursor);
    }

    public virtual void MouseOff()
    {
        EmitSignal(nameof(MousedOff));
    }
}
