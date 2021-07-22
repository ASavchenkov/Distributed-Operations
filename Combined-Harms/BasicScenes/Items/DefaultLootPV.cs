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
        M1.Connect(nameof(ClickDragTracker.Drag), this, nameof(OnDrag));
    }

    public void OnDrag()
    {
        GD.Print("Dragging");
    }

    public virtual void Subscribe( object _provider)
    {
        provider = (ILootItem) _provider;
    }


}