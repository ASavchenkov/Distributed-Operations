using Godot;
using System;

using ReplicationAbstractions;

public class DefaultLootPV : DraggableArea, IObserver
{

    public virtual ILootItem provider {get; protected set;}
    public LootSlotObserver parent;
    
    [Export]
    public float Radius = 1; //For automatic spacing.

    [Signal]
    public delegate void RequestPosReset();

    public override void _Ready()
    {
        base._Ready();
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
    }

    public virtual void Subscribe( object _provider)
    {
        provider = (ILootItem) _provider;
    }

    public override void OnMouseUpdate()
    {
        foreach(Spatial intersected in menu.mouseIntersections)
        {
            if(intersected is InventoryWorkspace area)
            {
                Translation = ToLocal(menu.intersectionPoints[intersected]);
                break;
            }
        }
    }

    public override void OnDrop()
    {
        base.OnDrop();
        //Currently resets when it's dropped
        //but in the future will check for loot slots
        //And special items that take dropped items.
        parent.RecomputeOccupantPos();
    }


}