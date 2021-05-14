using Godot;
using System;

using ReplicationAbstractions;

public class LootSlotObserver : Area
{

    public LootSlot Slot;
    public Spatial observer = null;


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
        
        OnTranslationSet(Slot.Translation);
    }

    public void OnOccupantSet(Node n)
    {
        if(observer != null)
            observer.QueueFree();

        if(n != null)
        {
            // observer = (Spatial) EasyInstancer.GenObserver(n, ((ILootPV)n).ObserverPathLootPV);
            // AddChild(observer);
        }
    }
    public void OnTranslationSet(Vector3 t)
    {
        GD.Print("trying to set translation");
        Translation = t;
        GD.Print("set translation successfully");
    }


    public void MouseOn()
    {
        GD.Print(Name, ": Moused on");
    }

    public void MouseOff()
    {
        GD.Print(Name, ": Moused off");
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