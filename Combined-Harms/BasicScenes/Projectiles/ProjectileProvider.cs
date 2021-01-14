using Godot;
using System;
using System.Collections.Generic;

/*
Base class for small fast moving objects
that do something when they hit something else.

These never change masters.
If the master disappears, so does the projectile.
*/

public class ProjectileProvider : Node, IReplicable, IFPV, I3PV
{
    //Replicable boilerplate
    [Export]
    public string ScenePath {get;set;}
    public HashSet<int> Unconfirmed {get;set;}
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
        ((IReplicable) this).ready();
    }

    public virtual void DefaultImpact()
    {
        //default functionality is to simply delete self
        //We need to make sure this deletion happens on all peers.
        Rpc(nameof(IReplicable.Despawn));
    }

    // public void ComputeImpact(BallisticTarget target)
    // {
    //     ImpactFunction matchedFunction;
    //     if(target is null)
    //         DefaultImpact();
    //     else if ( ImpactFunctions.TryGetValue(target.GetType(), out matchedFunction))
    //         matchedFunction(target);
    //     else if (!target.ComputeImpact(this))
    //         DefaultImpact();
    // }

    [PuppetSync]
    public void UpdateTrajectory(Vector3 translation, Vector3 velocity)
    {
        Translation = translation;
        LinearVelocity = velocity;
    }

    //The base function handles hit detection
    //Additional ballistics are left to the engine
    //as well as the deriving class.
    

}
