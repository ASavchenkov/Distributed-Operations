using Godot;
using System;

public class Player : Node
{
    
    Spatial LookYaw;
    Spatial LookPitch;
    RigidBody Body;

    [Export]
    int mouseSensitivity = 100;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LookYaw = (Spatial) GetNode("LookYaw");
        LookPitch = (Spatial) GetNode("LookYaw/LookPitch");
        Body = (RigidBody) GetNode("Body");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseEvent)
        {
            //Yes these look flipped. It's correct.
            LookYaw.RotateY(-mouseEvent.Relative.x/mouseSensitivity);
            LookPitch.RotateX(-mouseEvent.Relative.y/mouseSensitivity);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        handleStrafing();
    }

    private void handleStrafing()
    {
        Vector3 desiredMove = new Vector3();

        //Add all the WASD controls to get a vector.
        if(Input.IsActionPressed("MoveForward"))
            desiredMove += Vector3.Forward;
        else if(Input.IsActionPressed("MoveLeft"))
            desiredMove += Vector3.Left;
        else if(Input.IsActionPressed("MoveBack"))
            desiredMove += Vector3.Back;
        else if(Input.IsActionPressed("MoveRight"))
            desiredMove += Vector3.Right;
        
        //what's the behavior of Normalized() when desiredMove is zero?
        desiredMove = desiredMove.Normalized();
        
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
