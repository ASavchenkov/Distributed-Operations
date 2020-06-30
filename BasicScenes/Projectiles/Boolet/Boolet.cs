using Godot;
using System;
using System.Collections.Generic;


public class Boolet : LocalProjectile
{

    public void hitBallisticTarget(BallisticTarget target)
    {
        target.Rpc("Hit");
        base.DefaultImpact();
    }

    public Boolet()
    {
        impactFunctions.Add(typeof(BallisticTarget), new ImpactFunction(hitBallisticTarget));
    }

    public void Init(Vector3 _velocity, Vector3 translation)
    {
        this.LinearVelocity = _velocity;
        this.Translation = translation;
        
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
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
