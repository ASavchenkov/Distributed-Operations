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

public interface ILootItem : IHasLootPV
{
    LootSlot parent {get;set;}
    bool Validate(ILootItem item, object stateUpdate);
}