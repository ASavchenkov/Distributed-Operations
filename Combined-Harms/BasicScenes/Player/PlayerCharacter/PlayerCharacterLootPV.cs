using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacterLootPV : DefaultLootPV
{
    
    private PlayerCharacterProvider _provider;
    public override ILootItem provider
    {
        get => _provider;
        protected set {_provider = (PlayerCharacterProvider) value;}
    }

    public override void Subscribe(object p)
    {
        provider = (ILootItem) p;
        ((LootSlotObserver) GetNode("ChestSlot")).Subscribe(_provider.ChestSlot);
        ((LootSlotObserver) GetNode("HandSlot")).Subscribe(_provider.HandSlot);
    }

}
