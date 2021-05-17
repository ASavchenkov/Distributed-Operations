using Godot;
using System;

using ReplicationAbstractions;

public class LootSlotObserver : Area, ILootPickable
{

    public LootSlot Slot;
    public Spatial observer = null;
    public PickingMember pMember {get; set;}

    public void Subscribe(LootSlot slot)
    {

        Slot = slot;
        slot.Connect(nameof(LootSlot.OccupantSet), this, nameof(OnOccupantSet));
        slot.Connect(nameof(LootSlot.TranslationSet), this, nameof(OnTranslationSet));
        
        if(!(Slot.Occupant is null))
        {
            GD.Print("occupant not null");
            OnOccupantSet(Slot.Occupant);
        }
        
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(Slot.Occupant is null)
            Slot.Translation = Translation;
        else
            OnTranslationSet(Slot.Translation);

        pMember = new PickingMember(this, this);
    }

    public void OnOccupantSet(Node n)
    {
        if(observer != null)
            observer.QueueFree();

        if(n != null)
        {
            observer = (Spatial) EasyInstancer.GenObserver(n, ((IHasLootPV)n).ObserverPathLootPV);
            AddChild(observer);
        }
    }
    public void OnTranslationSet(Vector3 t)
    {
        GD.Print("trying to set translation");
        Translation = t;
        GD.Print("set translation successfully");
    }

}

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
public interface ILootPickable
{
    PickingMember pMember {get; set;}
}

//Inherits Godot.Object to get access to signals.
public class PickingMember : Godot.Object
{
    private Spatial owner;
    private Area area;
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

    public void UpdateTranslation(Vector3 t)
    {
        owner.Translation = t;
    }

    public void MouseOn()
    {
        GD.Print(owner.Name, ": Moused on");
    }

    public void MouseOff()
    {
        GD.Print(owner.Name, ": Moused off");
    }
}
