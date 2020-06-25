using Godot;
using System;
using System.Collections.Generic;
/*
Has information for bullets to compute their continued trajectory
(or lack thereof)

If a bullet hits a collider without one of these, it is destroyed.
That is to say: anything that will definitely just eat a bullet
doesn't need ballistic properties. Eg:

the ground
huge rocks.
Really thick walls.

also emits a signal for when the impact changes game state, eg:
player hp
spray location.
*/
public class BallisticCollider : Area
{

    public Dictionary<Type,Action<Projectile>> impactFunctions
     = new Dictionary<Type,Action<Projectile>>();
    // Check "Projectile" for explanation as to what this does.
    // This is essentially the mirror of that.


    [Signal]
    public delegate void Hit();
    //The default signal that projectiles or BallisticColliders can trigger.


    public virtual bool ComputeImpact(Projectile projectile)
    {
       Action<Projectile> matchedAction;
        if ( impactFunctions.TryGetValue(projectile.GetType(), out matchedAction))
        {
            matchedAction(projectile);
            return true;
        }
        else return false;
        //If even we don't know what to do with it, tell it to do its default.
    }
}
