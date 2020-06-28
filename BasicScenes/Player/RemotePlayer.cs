using Godot;
using System;

public class RemotePlayer : Player
{

    public override void _Ready()
    {
        base._Ready();
    }

    [Puppet]
    public void UpdateTrajectory(Vector3 translation, Vector3 yaw, Vector3 pitch)
    {
        Body.Translation = translation;
        LookYaw.Rotation = yaw;
        LookPitch.Rotation = pitch;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
