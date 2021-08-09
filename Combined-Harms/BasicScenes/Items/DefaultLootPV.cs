using Godot;
using System;

using ReplicationAbstractions;

public class DefaultLootPV : DraggableArea, IObserver
{

    public virtual ILootItem provider {get; protected set;}
    public Spatial parent;
    
    [Export]
    public float Radius = 1; //For automatic spacing.

    [Signal]
    public delegate void RequestPosReset();

    public virtual void Subscribe( object _provider)
    {
        provider = (ILootItem) _provider;
    }

    public override void _Ready()
    {
        base._Ready();
        parent = GetParentSpatial();
    }

    public override void OnMouseUpdate()
    {
        foreach(Spatial intersected in menu.mouseIntersections)
        {
            if(intersected.Name == "InventoryWorkspace")
            {
                GD.Print(menu.intersectionPoints[intersected]);
                Translation = parent.ToLocal(menu.intersectionPoints[intersected]);
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
        if(parent is LootSlotObserver lootSlotObserver)
            lootSlotObserver.RecomputeOccupantPos();
    }


}