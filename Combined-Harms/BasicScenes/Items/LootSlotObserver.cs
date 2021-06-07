using Godot;
using System;

using ReplicationAbstractions;

//Huh why isn't this an IObserver?
//Because it doesn't need the whole generic "Subscribe"
//function that IObserver has for the GenObserver function.
public class LootSlotObserver : PickableArea, IAcceptsDrop
{
    public LootSlot Slot;
    public DefaultLootPV OccupantObserver = null;

    
    public void Subscribe(LootSlot slot)
    {
        Slot = slot;
        slot.Connect(nameof(LootSlot.OccupantSet), this, nameof(OnOccupantSet));
        
        if(!(Slot.Occupant is null))
        {
            GD.Print("occupant not null");
            OnOccupantSet(Slot.Occupant);
        }
        
        slot.Connect(nameof(LootSlot.TransformSet), this, nameof(SetLTransform));
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(Slot.Occupant is null)
            Slot.Transform = Transform;
        else
            SetLTransform(Slot.Transform);
    }

    public void OnOccupantSet(Node n)
    {
        GD.Print("setting occupant: ", n?.Name);
        (OccupantObserver as Node)?.QueueFree();
        OccupantObserver = null;

        if(n != null)
        {
            OccupantObserver = (DefaultLootPV) EasyInstancer.GenObserver(n, ((IHasLootPV)n).ObserverPathLootPV);
            AddChild( (Node) OccupantObserver);
            OccupantObserver.parent = this;
            RecomputeOccupantPos();
        }
    }

    public void SetLTransform(Transform localTarget)
    {
        Transform = localTarget;
        RecomputeOccupantPos();
    }

    //Makes sure that the observer is placed the appropriate distance from the handle.
    public override void SetGTransform(Transform globalTarget)
    {
        GlobalTransform = globalTarget;
        RecomputeOccupantPos();
    }

    public void RecomputeOccupantPos()
    {
        if(!(OccupantObserver is null))
            OccupantObserver.Translation = Translation.Normalized() * OccupantObserver.Radius;
    }

    //accepts null item
    //Because that's how Dropping removes things from other spots.
    public bool Drop(DefaultLootPV item)
    {
        GD.Print(Slot.Occupant);
        //Drop function should have integral validation code.
        //Don't accept drops that are obviously not possible.
        if(!(Slot.Occupant is null || item is null))
        {
            GD.Print("cannot drop non-null item into occupied slot");
            return false;
        }

        Slot.Occupant = item?.provider;
        item?.parent?.Drop(null);

        GD.Print("LootSlotObserver dropped successfully");
        return true;
    }
}






