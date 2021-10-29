using Godot;
using System;

using ReplicationAbstractions;

public class DefaultInvPV : DraggableSpatial, IObserver
{

    public virtual IInvItem Provider {get; protected set;}
    public Spatial parent;
    
    [Export]
    public float Radius = 1; //For automatic spacing.

    [Signal]
    public delegate void RequestPosReset();

    public virtual void Subscribe( object _provider)
    {
        Provider = (IInvItem) _provider;
    }

    public override void _Ready()
    {
        base._Ready();
        parent = GetParentSpatial();
    }

    public override void OnCursorUpdate()
    {
        foreach(Spatial intersected in cursor.mouseIntersections)
        {
            if(intersected.Name == "InventoryWorkspace")
            {
                Translation = parent.ToLocal(cursor.intersectionPoints[intersected]);
                break;
            }
        }
    }

    public override void OnDrop()
    {

        foreach(Spatial intersection in cursor.mouseIntersections)
        {
            GD.Print(intersection.Name, ", ", intersection is IAcceptsItem);
            if(intersection is IAcceptsItem acceptor)
            {
                if(acceptor.AcceptItem(this))
                {
                    QueueFree();
                    return;
                }
                else break; //Don't want unpredictable dropping into unseen acceptors.
            }
        }

        //Only do this if not accepted.
        //Since Accepted drops result in QueueFreeing this node anyways.

        base.OnDrop();
        if(parent is InvSlotObserver invSlotObserver)
            invSlotObserver.RecomputeOccupantPos();
    }


}