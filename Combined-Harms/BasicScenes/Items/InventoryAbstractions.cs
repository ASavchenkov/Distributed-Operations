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

    private Vector3 _Translation = new Vector3();
    public Vector3 Translation 
    {
        get { return _Translation;}
        set 
        {
            _Translation = value;
            EmitSignal(nameof(TranslationSet), value);
        }
    }
    [Signal]
    public delegate void OccupantSet(Node n);
    [Signal]
    public delegate void TranslationSet(Vector3 pos);

}

//Interface to let the InventoryMenu interact with these things
//clicking on them and moving them around.
//All functionality implemented in PickingMember
public interface ILootPickable
{
    PickingMember pMember {get; set;}
}

//Inherits Godot.Object to get access to signals.
public class PickingMember : Godot.Object
{
    protected Spatial owner;
    protected Area area;
    public PickingMember(Spatial _owner, Area _area)
    {
        owner = _owner;
        area = _area;
    }

    //When we're clicked on but not yet clicked off
    //assume being dragged, since dragging can be very fast.
    public void Press(InventoryMenu menu)
    {
        menu.Connect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateTranslation));
        
        //functionally disable so the ray can look at where it's gonna drop stuff
        //and not get blocked by us. We will enable it again when we're done being held.
        area.InputRayPickable = false;
    }

    public void Release(InventoryMenu menu)
    {
        menu.Disconnect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateTranslation));
        area.InputRayPickable = true;
    }
    
    public void UpdateTranslation(Vector3 t)
    {
        owner.Translation = t;
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

public interface ILootPV : ILootPickable
{
    void FullClick();
}

public interface IAcceptsDrop
{
    void Drop( ILootPV item);
}