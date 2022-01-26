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


    public override void OnCursorUpdate()
    {
        //Come on. We can do better for searching right?
        foreach(IPickable intersected in cursor.mouseIntersections)
        {
            if(((Spatial)intersected).Name == "InventoryWorkspace")
            {
                //Something has gone horribly wrong if the parent is not a Spatial.
                //Intentionally crash if GetParentSpatial returns null;
                //Protects against silent failure down the development chain.
                Translation = GetParentSpatial().ToLocal(cursor.intersectionPoints[intersected]);
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
        if(GetParent() is InvSlotObserver invSlotObserver)
            invSlotObserver.RecomputeOccupantPos();
    }


}