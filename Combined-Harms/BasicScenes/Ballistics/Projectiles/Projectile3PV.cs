using Godot;
using System;

using ReplicationAbstractions;

public class Projectile3PV : RigidBody, IObserver
{
    
    //Whoah this is confusing right?
    //Well I want public gets and private sets... and this is the only way to do that*.
    //*(that I know of)
    protected ProjectileProvider _provider;
    public ProjectileProvider provider {get => _provider; private set => _provider = value;}

    public void Subscribe(Node provider)
    {
        this.provider = (ProjectileProvider) provider;
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
