using Godot;
using System;
using System.Collections.Generic;

/*
Base class for small fast moving objects
that do something when they hit something else.

These never change masters.
If the master disappears, so does the projectile.
*/

public class ProjectileProvider : Node, IReplicable, IFPV
{
    //Replicable boilerplate
    [Export]
    public string ScenePath {get;set;}
    public HashSet<int> Unconfirmed {get;set;}
    //IFPV boilerplate
    [Export]
    public string ObserverPathFPV {get;set;}

    public delegate void ImpactFunction( BallisticTarget target);

    public Dictionary<Type,ImpactFunction> impactFunctions
     = new Dictionary<Type,ImpactFunction>();
    //This looks super complicated, but it's only a little bit complicated.
    //It's a dictionary that maps specific BallisticTargets 
    //to functions that compute interaction with them.

    public Vector3 Translation;
    public Vector3 LinearVelocity;

    [PuppetSync]
    public void Init(Vector3 Translation, Vector3 LinearVelocity)
    {
        this.Translation = Translation;
        this.LinearVelocity = LinearVelocity;
        
        //Currently everyone uses the same observer,
        //so we don't need to do the whole if else shebang.
        ProjectileFPV observer = (ProjectileFPV) EasyInstancer.GenObserver(this, ObserverPathFPV);
        GetNode("/root/GameRoot/Map").AddChild(observer);

        //This is normally handled by differing observer code.
        //But for now it's not worth it.
        if(! IsNetworkMaster())
            observer.Mode = RigidBody.ModeEnum.Kinematic;
    }

    public override void _Ready()
    {
        ((IReplicable) this).ready();
    }

    [PuppetSync]
    public void Despawn()
    {
        QueueFree();
    }

    public virtual void DefaultImpact()
    {
        //default functionality is to simply delete self
        //We need to make sure this deletion happens on all peers.
        Rpc("Despawn");
    }



    public void ComputeImpact(BallisticTarget target)
    {
        ImpactFunction matchedFunction;
        if(target is null)
            DefaultImpact();
        else if ( impactFunctions.TryGetValue(target.GetType(), out matchedFunction))
            matchedFunction(target);
        else if (!target.ComputeImpact(this))
            DefaultImpact();
    }

    [Puppet]
    public void UpdateTrajectory(Vector3 translation, Vector3 velocity)
    {
        Translation = translation;
        LinearVelocity = velocity;
        rayCast.CastTo = LinearVelocity /60 * 20.0F;
            
    }

    //The base function handles hit detection
    //Additional ballistics are left to the engine
    //as well as the deriving class.
    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        //If we're in control of the projectile, do all the ballistics
        if(IsNetworkMaster())
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
                ComputeImpact(target);
            }
            Rpc("UpdateTrajectory", Translation, state.LinearVelocity);
        }
    }

}
