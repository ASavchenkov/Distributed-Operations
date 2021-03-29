using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class BooletFPV : ProjectileFPV
{
    public new BooletProvider provider {get => (BooletProvider) _provider; private set => _provider = value;}

    public override void OnContact(IBallisticTarget target)
    {
        //Just delete ourselves when we hit something.
        //It should have already handled damage and stuff.
        GD.Print("Boolet hit something");
        provider.rMember.MasterDespawn();
    }
}
