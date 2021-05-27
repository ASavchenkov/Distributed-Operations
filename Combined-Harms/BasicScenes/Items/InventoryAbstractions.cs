using Godot;
using System;

//A Godot.Object that alerts others when its occupant or positon is changed.
//Used on providers to store what loot is in their slots.
public class LootSlot : Godot.Object
{
    private Node _Occupant = null;
    public Node Occupant 
    {
        get { return _Occupant;}
        set 
        {
            _Occupant = value;
            EmitSignal(nameof(OccupantSet), value);
        }
    }

    private Transform _Transform = new Transform();
    public Transform Transform 
    {
        get { return _Transform;}
        set 
        {
            _Transform = value;
            EmitSignal(nameof(TransformSet), value);
        }
    }
    [Signal]
    public delegate void OccupantSet(Node n);
    [Signal]
    public delegate void TransformSet(Transform t);

}

public abstract class PickableArea : Area
{
    //When we're clicked on but not yet clicked off
    //assume being dragged, since dragging can be very fast.
    public virtual void Press(InventoryMenu menu)
    {
        menu.Connect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateTransform));
        
        //functionally disable so the ray can look at where it's gonna drop stuff
        //and not get blocked by us. We will enable it again when we're done being held.
        InputRayPickable = false;
    }

    public virtual void Release(InventoryMenu menu)
    {
        menu.Disconnect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateTransform));
        InputRayPickable = true;
    }

    public virtual void MouseOn()
    {
        GD.Print(Name, ": Moused on");
    }

    public virtual void MouseOff()
    {
        GD.Print(Name, ": Moused off");
    }
    public abstract void UpdateTransform(Transform globalTarget);
}

public interface IAcceptsDrop
{
    void Drop( DefaultLootPV item);
}