using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public abstract class ProjectileFPV : RigidBody, IObserver
{

    protected ProjectileProvider _provider;

    //"new this property and cast to appropriate type in deriving classes.
    public ProjectileProvider provider {get => _provider; private set => _provider = value;}
    
    public RayCast rayCast;
    
    public void Subscribe(Node provider)
    {
        this.provider = (ProjectileProvider) provider;
        Translation = this.provider.LastTranslation;
        LinearVelocity = this.provider.LastLinearVelocity;
        this.DefaultSubscribe(this.provider);
    }

    public override void _Ready()
    {
        rayCast = (RayCast) GetNode("RayCast");
    }

    public virtual void OnContact(IBallisticTarget target)
    {
        GD.Print("base OnContact");
        //Does nothing by default.

    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        
        float timeLeft = state.Step;
        
        rayCast.CastTo = state.LinearVelocity * timeLeft * 20.0F;
        rayCast.ForceRaycastUpdate();
        
        if(rayCast.IsColliding())
        {

            //GetCollider will never return null since IsColliding() returned true
            IBallisticTarget target = rayCast.GetCollider() as IBallisticTarget;
            //But target can be null if it's not a BallisticTarget
            if(IsInstanceValid((Node) target))
                target.OnContact(this);
            OnContact(target);
        }
    }
}