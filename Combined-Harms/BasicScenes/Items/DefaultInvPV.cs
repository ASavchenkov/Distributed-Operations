using Godot;
using System;

using ReplicationAbstractions;

public class DefaultInvPV : DraggableArea, IObserver
{

    public virtual IInvItem provider {get; protected set;}
    public Spatial parent;
    
    [Export]
    public float Radius = 1; //For automatic spacing.

    [Signal]
    public delegate void RequestPosReset();

    public virtual void Subscribe( object _provider)
    {
        provider = (IInvItem) _provider;
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
                Translation = parent.ToLocal(menu.intersectionPoints[intersected]);
                break;
            }
        }
    }

    public override void OnDrop()
    {

        foreach(Spatial intersection in menu.mouseIntersections)
        {
            if(intersection is IAcceptsItem acceptor)
                if(acceptor.AcceptItem(this))
                    return;
                else break; //Don't want unpredictable dropping into unseen acceptors.
        }

        //Only do this if not accepted.
        //Since Accepted drops result in QueueFreeing this node anyways.

        base.OnDrop();
        if(parent is InvSlotObserver invSlotObserver)
            invSlotObserver.RecomputeOccupantPos();
    }


}