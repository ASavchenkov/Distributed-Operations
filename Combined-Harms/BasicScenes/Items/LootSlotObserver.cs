using Godot;
using System;

using ReplicationAbstractions;

//Huh why isn't this an IObserver?
//Because it doesn't need the whole generic "Subscribe"
//function that IObserver has for the GenObserver function.
public class LootSlotObserver : IPickable, IAcceptsDrop
{
    public LootSlot provider;
    public DefaultLootPV OccupantObserver = null;

    
    public void Subscribe(LootSlot slot)
    {
        provider = slot;
        slot.Connect(nameof(LootSlot.OccupantSet), this, nameof(OnOccupantSet));
        
        if(!(provider.Occupant is null))
        {
            GD.Print("occupant not null");
            OnOccupantSet((Node) provider.Occupant);
        }
        
        slot.Connect(nameof(LootSlot.TransformSet), this, nameof(SetLTransform));
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(provider.Occupant is null)
            provider.Transform = Transform;
        else
            SetLTransform(provider.Transform);
    }

    public void OnOccupantSet(Node n)
    {
        GD.Print(Name, "  is setting occupant: ", n?.Name);
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

    //Purely a way for InventoryMenu to interact with provider drop function
    public void Drop(DefaultLootPV item)
    {
        provider.Drop(item.provider);
    }
}






