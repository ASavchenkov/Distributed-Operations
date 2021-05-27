using Godot;
using System;

using ReplicationAbstractions;

public class LootSlotObserver : PickableArea, IAcceptsDrop
{
    public LootSlot Slot;
    public DefaultLootPV observer = null;

    
    public void Subscribe(LootSlot slot)
    {
        Slot = slot;
        slot.Connect(nameof(LootSlot.OccupantSet), this, nameof(OnOccupantSet));
        
        if(!(Slot.Occupant is null))
        {
            GD.Print("occupant not null");
            OnOccupantSet(Slot.Occupant);
        }
        
        slot.Connect(nameof(LootSlot.TransformSet), this, nameof(UpdateTransform));
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(Slot.Occupant is null)
            Slot.Transform = Transform;
        else
            UpdateTransform(Slot.Transform);
    }

    public void OnOccupantSet(Node n)
    {
        GD.Print("setting occupant: ", n?.Name);
        (observer as Node)?.QueueFree();

        if(n != null)
        {
            observer = (DefaultLootPV) EasyInstancer.GenObserver(n, ((IHasLootPV)n).ObserverPathLootPV);
            AddChild( (Node) observer);
            RecomputeObserverPos();
        }
    }
    public void Drop(DefaultLootPV item)
    {
        GD.Print("LootSlotObserver drop");
    }

    //Makes sure that the observer is placed the appropriate distance from the handle.
    public override void UpdateTransform(Transform globalTarget)
    {
        GlobalTransform = globalTarget;
        RecomputeObserverPos();
    }
    private void RecomputeObserverPos()
    {
        if(!(observer is null))
            observer.Translation = Translation.Normalized() * observer.Radius;
    }

}






