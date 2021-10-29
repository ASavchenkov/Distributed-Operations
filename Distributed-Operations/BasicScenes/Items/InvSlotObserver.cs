using Godot;
using System;

using ReplicationAbstractions;

//Huh why isn't this an IObserver?
//Because it doesn't need the whole generic "Subscribe"
//function that IObserver has for the GenObserver function.
public class InvSlotObserver : DraggableSpatial, IAcceptsItem
{
    public InvSlot provider;
    public DefaultInvPV OccupantObserver = null;

    
    public void Subscribe(InvSlot slot)
    {
        provider = slot;
        slot.Connect(nameof(InvSlot.OccupantSet), this, nameof(OnOccupantSet));
        
        if(!(provider.Occupant is null))
        {
            OnOccupantSet((Node) provider.Occupant);
        }
        
        slot.Connect(nameof(InvSlot.TranslationSet), this, nameof(SetLTranslation));
        //Default position is where it is in this scene.
        //Otherwise, configure ourselves based on the Slot.
        if(provider.Translation.HasValue)
            SetLTranslation(provider.Translation.Value);
        else
            provider.Translation = Translation;
    }

    public void OnOccupantSet(Node n)
    {
        GD.Print(Name, "  is setting occupant: ", n?.Name);
        (OccupantObserver as Node)?.QueueFree();
        OccupantObserver = null;

        if(n != null)
        {
            OccupantObserver = (DefaultInvPV) EasyInstancer.GenObserver(n, ((IHasInvPV)n).ObserverPathInvPV);
            AddChild( (Node) OccupantObserver);
            OccupantObserver.parent = this;
            RecomputeOccupantPos();
        }
    }

    public override void OnCursorUpdate()
    {
        foreach(Spatial intersected in cursor.mouseIntersections)
        {
            if(intersected.Name == "InventoryWorkspace")
            {
                SetLTranslation(((Spatial)GetParent()).ToLocal(cursor.intersectionPoints[intersected]));
                break;
            }
        }
    }

    public void SetLTranslation(Vector3 localTarget)
    {
        Translation = localTarget;
        RecomputeOccupantPos();
    }

    public void RecomputeOccupantPos()
    {
        if(!(OccupantObserver is null))
            OccupantObserver.Translation = Translation.Normalized() * OccupantObserver.Radius;
    }

    //Purely a way for InventoryMenu to interact with provider Accept function
    public bool AcceptItem(DefaultInvPV item)
    {
        return provider.AcceptItem(item.Provider);
    }
}






