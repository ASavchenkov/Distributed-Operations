using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacterLootPV : DefaultLootPV
{
    
    private PlayerCharacterProvider _provider;
    public override Node provider
    {
        get => _provider;
        protected set {_provider = (PlayerCharacterProvider) value;}
    }

    public override void Subscribe(object p)
    {
        provider = (Node) p;
        ((LootSlotObserver) GetNode("ChestSlot")).Subscribe(_provider.ChestItem);
        ((LootSlotObserver) GetNode("HandSlot")).Subscribe(_provider.HandItem);
    }

}
