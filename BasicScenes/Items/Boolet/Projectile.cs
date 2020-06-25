using Godot;
using System;
using System.Collections.Generic;
/*
Base class for small fast moving objects
that do something when they hit something else.
*/
public class Projectile : RigidBody
{
    
    public delegate void ImpactFunction( BallisticCollider target);

    public Dictionary<Type,ImpactFunction> impactFunctions
     = new Dictionary<Type,ImpactFunction>();
    //This looks super complicated, but it's only a little bit complicated.
    //It's a dictionary that maps specific BallisticColliders 
    //to functions that compute interaction with them.

    public RayCast rayCast;

    public override void _Ready()
    {
        rayCast = (RayCast) GetNode("RayCast");
    }

    public virtual void DefaultImpact()
    {
        //default functionality is to simply delete self
        QueueFree();
    }

    public void ComputeImpact(BallisticCollider target)
    {
        ImpactFunction matchedFunction;
        if(target is null)
            DefaultImpact();
        else if ( impactFunctions.TryGetValue(target.GetType(), out matchedFunction))
            matchedFunction(target);
        else if (!target.ComputeImpact(this))
            DefaultImpact();
    }

    //The base function handles hit detection
    //Additional ballistics are left to the engine
    //as well as the deriving class.
    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        float timeLeft = state.Step;
        
        rayCast.CastTo = state.LinearVelocity * timeLeft * 20.0F;
        rayCast.ForceRaycastUpdate();
        
        if(rayCast.IsColliding())
        {

            //GetCollider will never return null since IsColliding() returned true
            BallisticCollider target = rayCast.GetCollider() as BallisticCollider;
            //But target can be null if it's not a BallisticCollider
            ComputeImpact(target);
        }
    }

}
