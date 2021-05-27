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

//Interface to let the InventoryMenu interact with these things
//clicking on them and moving them around.
//All functionality implemented in PickingMember
public interface ILootPickable
{
    PickingMember pMember {get;}
    void UpdateTransform(Transform globalTarget);
}

//Inherits Godot.Object to get access to signals.
public class PickingMember : Godot.Object
{
    protected Spatial owner;
    protected Area area;
    
    [Signal]
    public delegate void Released();
    
    public PickingMember(Spatial _owner, Area _area)
    {
        owner = _owner;
        area = _area;
    }

    //When we're clicked on but not yet clicked off
    //assume being dragged, since dragging can be very fast.
    public virtual void Press(InventoryMenu menu)
    {
        menu.Connect(nameof(InventoryMenu.RayUpdated), owner, nameof(ILootPickable.UpdateTransform));
        
        //functionally disable so the ray can look at where it's gonna drop stuff
        //and not get blocked by us. We will enable it again when we're done being held.
        area.InputRayPickable = false;
    }

    public virtual void Release(InventoryMenu menu)
    {
        menu.Disconnect(nameof(InventoryMenu.RayUpdated), owner, nameof(ILootPickable.UpdateTransform));
        area.InputRayPickable = true;
    }

    public virtual void MouseOn()
    {
        GD.Print(owner.Name, ": Moused on");
    }

    public virtual void MouseOff()
    {
        GD.Print(owner.Name, ": Moused off");
    }
}

public interface IAcceptsDrop
{
    void Drop( DefaultLootPV item);
}