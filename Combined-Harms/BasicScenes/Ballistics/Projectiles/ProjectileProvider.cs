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

public class ProjectileProvider : Node, IReplicable, IFPV, I3PV
{

    //Replicable boilerplate
    public ReplicationMember rMember {get; set;}
    [Export]
    public string ScenePath {get;set;}
    //Observer boilerplate
    [Export]
    public string ObserverPathFPV {get;set;}
    [Export]
    public string ObserverPath3PV {get;set;}


    public Vector3 Translation;
    public Vector3 LinearVelocity;

    [PuppetSync]
    public void Init(Vector3 Translation, Vector3 LinearVelocity)
    {
        this.Translation = Translation;
        this.LinearVelocity = LinearVelocity;
        
        Node observer = EasyInstancer.GenObserver(this, (IsNetworkMaster()) ?  ObserverPathFPV: ObserverPath3PV);
        GetNode("/root/GameRoot/Map").AddChild(observer);

    }

    public override void _Ready()
    {
        this.ReplicableReady();
    }

    [PuppetSync]
    public void UpdateTrajectory(Vector3 translation, Vector3 velocity)
    {
        Translation = translation;
        LinearVelocity = velocity;
    }
}
