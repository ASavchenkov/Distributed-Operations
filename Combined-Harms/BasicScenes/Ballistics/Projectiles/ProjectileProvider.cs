using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;
/*
Base class for small fast moving objects
that do something when they hit something else.

These never change masters.
If the master disappears, so does the projectile.
*/

public class ProjectileProvider : Node, IReplicable, IHasFPV, IHas3PV
{

    //Replicable boilerplate
    public ReplicationMember rMember {get; set;}

    public Guid Gooeyd {get;set;}
    [Export]
    public string ScenePath {get;set;}
    //Observer boilerplate
    [Export]
    public string ObserverPathFPV {get;set;}
    [Export]
    public string ObserverPath3PV {get;set;}

    public Vector3 LastTranslation;
    public Vector3 LastLinearVelocity;


    [Signal]
    public delegate void TrajectoryUpdated( Vector3 translation, Vector3 velocity);

    [PuppetSync]
    public void Init(Vector3 translation, Vector3 velocity)
    {
        LastTranslation = translation;
        LastLinearVelocity = velocity;
        RigidBody observer = (RigidBody) EasyInstancer.GenObserver(this, (IsNetworkMaster()) ?  ObserverPathFPV: ObserverPath3PV);
        GetNode("/root/GameRoot/Map").AddChild(observer);
    }

    public override void _Ready()
    {
        this.ReplicableReady();
    }

    //TL:DR call this in the FPV observers overriden "OnContact" function
    //when something notable happens. 
    //we only update the trajectory when something of note happens to the projectile.
    //otherwise, the 3PV observer should estimate trajectory on it's own
    //to retain smooth visuals. (Projectile3PV is just a visualization after all.
    [Remote]
    public void UpdateTrajectory(Vector3 translation, Vector3 velocity)
    {
        LastTranslation = translation;
        LastLinearVelocity = velocity;
        EmitSignal(nameof(TrajectoryUpdated), translation, velocity);
    }
}
