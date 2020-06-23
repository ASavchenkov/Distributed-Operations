using Godot;
using System;

/*
Boolet doesn't use physics because all of its physics interactions
are specific to high velocity ballistics, which the physics engine 
is not designed for
*/
public class Boolet : Spatial
{

    public Vector3 velocity; 

    public void Init(Vector3 _velocity, Vector3 translation)
    {
        velocity = _velocity;
        GD.Print(velocity);
        this.Translation = translation;
        GD.Print(this.Translation);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        this.Translation += velocity * delta;
    }
}
