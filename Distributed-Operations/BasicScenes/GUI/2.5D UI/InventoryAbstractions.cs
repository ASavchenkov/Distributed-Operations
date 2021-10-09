using Godot;
using System;

using ReplicationAbstractions;

public interface IPickable : ITakesInput
{
    //For transparent colliders you place in the ui.
    //These take inputs non-permeable IPickables don't.
    //E.G: Scrolling when moused over an IPickable that
    //Doesn't consume scroll inputs.
    bool Permeable {get;set;}
    void MouseOn(MultiRayCursor cursor);
    void MouseOff();
}

//All InvItems are IReplicable and have InvPVs
public interface IInvItem : IReplicable, ISaveable, IHasInvPV
{
    InvSlot parent {get;set;}
    bool Validate(IInvItem item, object stateUpdate);
}

public interface IAcceptsItem
{
    bool AcceptItem( DefaultInvPV item);
}