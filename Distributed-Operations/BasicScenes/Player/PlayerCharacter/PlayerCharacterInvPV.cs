using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacterInvPV : DefaultInvPV
{
    
    private PlayerCharacterProvider _provider;
    public override IInvItem Provider
    {
        get => _provider;
        protected set {_provider = (PlayerCharacterProvider) value;}
    }

    public override void Subscribe(object p)
    {
        Provider = (IInvItem) p;
        ((InvSlotObserver) GetNode("ChestSlot")).Subscribe(_provider.ChestSlot);
        ((InvSlotObserver) GetNode("HandSlot")).Subscribe(_provider.HandSlot);
    }
}
