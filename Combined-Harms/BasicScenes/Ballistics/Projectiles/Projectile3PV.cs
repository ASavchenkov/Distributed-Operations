using Godot;
using System;

using ReplicationAbstractions;

public class Projectile3PV : RigidBody, IObserver
{
    
    public ProjectileProvider provider {get; private set;}

    public void Subscribe(object _provider)
    {
        provider = (ProjectileProvider) _provider;
        Translation = this.provider.LastTranslation;
        LinearVelocity = this.provider.LastLinearVelocity;
        
        provider.Connect(nameof(ProjectileProvider.TrajectoryUpdated), this, nameof(OnTrajectoryUpdated));
        this.DefaultSubscribe(this.provider);
    }

    public void OnTrajectoryUpdated( Vector3 translation, Vector3 velocity)
    {
        Translation = translation;
        LinearVelocity = velocity;
    }
}
