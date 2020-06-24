using Godot;
using System;

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

    [Export]
    public float KEDensity = 10;
    //how much energy does this object disappate per meter^3.

    [Signal]
    public delegate void Hit();

    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
