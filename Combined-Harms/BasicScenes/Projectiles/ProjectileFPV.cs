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
        ((IObserver)this).DefaultSub(this.provider);
        Translation = this.provider.Translation;
        LinearVelocity = this.provider.LinearVelocity;
    }

    public override void _Ready()
    {
        rayCast = (RayCast) GetNode("RayCast");
    }
}
