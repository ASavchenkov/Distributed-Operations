using Godot;
using System;
using System.Collections.Generic;

public class ProjectileFPV : RigidBody, IObserver
{

    public ProjectileProvider provider;
    
    public RayCast rayCast;
    
    public void Subscribe(Node provider)
    {
        this.provider = (ProjectileProvider) provider;
        ((IObserver)this).Subscribe(this.provider);
        Translation = this.provider.Translation;
        LinearVelocity = this.provider.LinearVelocity;
    }

    public override void _Ready()
    {
        rayCast = (RayCast) GetNode("RayCast");
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        
        float timeLeft = state.Step;
        
        rayCast.CastTo = state.LinearVelocity * timeLeft * 20.0F;
        rayCast.ForceRaycastUpdate();
        
        if(rayCast.IsColliding())
        {

            //GetCollider will never return null since IsColliding() returned true
            BallisticTarget target = rayCast.GetCollider() as BallisticTarget;
            GD.Print(rayCast.GetCollider().GetType());
            //But target can be null if it's not a BallisticTarget
            // provider.ComputeImpact(target);
        }

        provider.Rpc("UpdateTrajectory", Translation, state.LinearVelocity);
        
    }
    
}
