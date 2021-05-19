using Godot;
using System;

using ReplicationAbstractions;

public class LootSlotObserver : Area, ILootPickable, IAcceptsDrop
{
    public LootSlot Slot;
    public Spatial observer = null;
    public PickingMember pMember {get; set;}

    public void Subscribe(LootSlot slot)
    {

        Slot = slot;
        slot.Connect(nameof(LootSlot.OccupantSet), this, nameof(OnOccupantSet));
        
        if(!(Slot.Occupant is null))
        {
            GD.Print("occupant not null");
            OnOccupantSet(Slot.Occupant);
        }
        
        pMember = new PickingMember(this, this);
        slot.Connect(nameof(LootSlot.TranslationSet), this.pMember, nameof(PickingMember.UpdateTranslation));
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(Slot.Occupant is null)
            Slot.Translation = Translation;
        else
            pMember.UpdateTranslation(Slot.Translation);
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
    public void Drop(ILootPV item)
    {
        GD.Print("LootSlotObserver drop");
    }

}






