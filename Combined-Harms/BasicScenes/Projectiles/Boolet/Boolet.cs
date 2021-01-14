using Godot;
using System;
using System.Collections.Generic;


public class BooletProvider : ProjectileProvider
{
    

    [Puppet]
    public void Hit()
    {
        GD.Print("Got Hit RPC");
    }

    public void HitBallisticTarget(BallisticTarget target)
    {
        GD.Print("Specific version called");
        target.Rpc("Hit");
        base.DefaultImpact();
    }

   

    //This function might have something more,
    //like an explosion, but for now it just deletes itself.
    public override void DefaultImpact()
    {
        GD.Print("Terminal Effect");
        base.DefaultImpact();
    }
}
