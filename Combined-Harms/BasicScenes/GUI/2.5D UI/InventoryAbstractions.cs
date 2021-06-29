using Godot;
using System;

using ReplicationAbstractions;

public interface Pickable
{
    void MouseOn(TwoFiveDMenu menu);
    void MouseOff(TwoFiveDMenu menu);
}



public interface ILootItem : IHasLootPV
{
    LootSlot parent {get;set;}
    bool Validate(ILootItem item, object stateUpdate);
}