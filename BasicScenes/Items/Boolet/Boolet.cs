using Godot;
using System;

public class Boolet : Spatial
{

    public Vector3 direction;

    public void Init(Vector3 _direction, Vector3 translation)
    {
        direction = _direction;
        this.Translation = translation;
        GD.Print(this.Translation);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
