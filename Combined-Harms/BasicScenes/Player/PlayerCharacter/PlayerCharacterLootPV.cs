using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacterLootPV : DefaultLootPV, IObserver
{
    
    private PlayerCharacterProvider provider;

    public void Subscribe(Node _provider)
    {
        provider = (PlayerCharacterProvider) _provider;
        ((LootSlotObserver) GetNode("ChestSlot")).Subscribe(provider.ChestItem);
        ((LootSlotObserver) GetNode("HandSlot")).Subscribe(provider.HandItem);
    }

}
