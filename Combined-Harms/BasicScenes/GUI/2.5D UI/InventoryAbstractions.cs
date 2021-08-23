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
    void MouseOn(TwoFiveDMenu menu);
    void MouseOff();
}

public interface IInvItem : IHasInvPV
{
    InvSlot parent {get;set;}
    bool Validate(IInvItem item, object stateUpdate);

    #region NodeStuff
    NodePath GetPath();
    string Name {get;set;}
    #endregion
}

public interface IAcceptsItem
{
    bool AcceptItem( DefaultInvPV item);
}