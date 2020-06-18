using Godot;
using System;

public class Player : Node
{
    
    Spatial LookYaw;
    Spatial LookPitch;
    RigidBody Body;

    [Export]
    float mouseSensitivity = 100;
    [Export]
    float maxSpeed = 10;
    [Export]
    float acceleration = 10;
    [Export]
    float maxPitch = 80;

    private bool inputEnabled = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Body = (RigidBody) GetNode("Body");
        LookYaw = (Spatial) Body.GetNode("LookYaw");
        LookPitch = (Spatial) LookYaw.GetNode("LookPitch");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseEvent && inputEnabled)
        {
            //Yes these look flipped. It's correct.
            LookYaw.RotateY(-mouseEvent.Relative.x/mouseSensitivity);
            LookPitch.RotateX(-mouseEvent.Relative.y/mouseSensitivity);
            if(LookPitch.RotationDegrees.x > maxPitch)
                LookPitch.RotationDegrees = new Vector3(maxPitch,0,0);
            else if (LookPitch.RotationDegrees.x < -maxPitch)
                LookPitch.RotationDegrees = new Vector3(-maxPitch,0,0);
        }
    }

    //some input is handled in _PhysicsProcess
    //so we need a custom boolean
    public void setInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public override void _PhysicsProcess(float delta)
    {
        handleStrafing();
    }

    private void handleStrafing()
    {
        Vector3 desiredMove = new Vector3();

        //Add all the WASD controls to get a vector.

        
        if(inputEnabled)
        {
            if(Input.IsActionPressed("MoveForward"))
                desiredMove += Vector3.Forward;
            else if(Input.IsActionPressed("MoveLeft"))
                desiredMove += Vector3.Left;
            else if(Input.IsActionPressed("MoveBack"))
                desiredMove += Vector3.Back;
            else if(Input.IsActionPressed("MoveRight"))
                desiredMove += Vector3.Right;
        }
        //what's the behavior of Normalized() when desiredMove is zero?
        //I guess it's still zero?
        desiredMove = desiredMove.Normalized()*maxSpeed;

        //desiredMove is still in local space.
        //We want to convert it to global space.
        //but we still want it to not have any y component.
        Vector3 globalMove = LookYaw.GlobalTransform.basis.Xform(desiredMove);
        
        Body.AddCentralForce((globalMove-Body.LinearVelocity)*acceleration);

        
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
