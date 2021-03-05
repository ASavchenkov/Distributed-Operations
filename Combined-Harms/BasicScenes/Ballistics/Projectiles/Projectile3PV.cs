using Godot;
using System;

using ReplicationAbstractions;

public class Projectile3PV : RigidBody, IObserver
{
    
    protected ProjectileProvider _provider;
    public ProjectileProvider provider {get => _provider; private set => _provider = value;}

    public void Subscribe(Node provider)
    {
        this.provider = (ProjectileProvider) provider;
        this.DefaultSubscribe(this.provider);
        Translation = this.provider.Translation;
        LinearVelocity = this.provider.LinearVelocity;
    }


}
