using Godot;
using System;
using System.Collections.Generic;


public class Boolet : Projectile
{

    [Puppet]
    public void Hit()
    {
        GD.Print("Got Hit RPC");
    }

    public void hitBallisticTarget(BallisticTarget target)
    {
        target.Rpc("Hit");
        base.DefaultImpact();
    }

    public Boolet()
    {
        impactFunctions.Add(typeof(BallisticTarget), new ImpactFunction(hitBallisticTarget));
    }

    //This function might have something more,
    //like an explosion, but for now it just deletes itself.
    public override void DefaultImpact()
    {
        GD.Print("Terminal Effect");
        base.DefaultImpact();
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        base._IntegrateForces(state);
    }
}
