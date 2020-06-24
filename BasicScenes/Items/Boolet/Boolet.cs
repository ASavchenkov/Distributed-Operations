using Godot;
using System;
using System.Collections.Generic;


public class Boolet : RigidBody
{
    
    public RayCast rayCast;
    public MeshInstance castDirection;

    public void Init(Vector3 _velocity, Vector3 translation)
    {
        this.LinearVelocity = _velocity;
        this.Translation = translation;
        
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rayCast = (RayCast) GetNode("RayCast");
        castDirection = (MeshInstance) GetNode("CastDirection");
    }



    //This function might have something more,
    //like an explosion, but for now it just deletes itself.
    public void TerminalEffect()
    {
        GD.Print("Terminal Effect");
        QueueFree();
    }


    //a projectile might travel along a non-linear path in a single frame
    //depending on penetration and ricochets.
    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        float timeLeft = state.Step;
        
        rayCast.CastTo = state.LinearVelocity * timeLeft * 10.0F;
        castDirection.Translation = rayCast.CastTo;
        rayCast.ForceRaycastUpdate();
        
        if(rayCast.IsColliding())
        {
            //GetCollider will never return null since IsColliding() returned true
            //Thus null means it's not a
            BallisticCollider target = rayCast.GetCollider() as BallisticCollider;
            //If the collider is not a BallisticCollider
            //Then do the default thing.
            if (target is null)
                TerminalEffect();
            else
            {
                target.EmitSignal(nameof(BallisticCollider.Hit));
                TerminalEffect();
            }
        }
    }
}
