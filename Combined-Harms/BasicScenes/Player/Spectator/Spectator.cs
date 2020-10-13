using Godot;
using System;


//Doesn't need observer pattern
//since only one spectator exists.
public class Spectator : Spatial
{
    public Spatial LookYaw;
    public Spatial LookPitch;
    public Camera camera;
    
    private bool inputEnabled = true;
    
    [Export]
    float mouseSensitivity = 100;
    [Export]
    public float maxPitch {get; private set;} = 80; //degrees
    [Export]
    public float maxSpeed {get; private set;} = 10;
    

    public override void _Ready()
    {
        LookYaw = (Spatial) GetNode("LookYaw");
        LookPitch = (Spatial) LookYaw.GetNode("LookPitch");
        camera = (Camera) LookPitch.GetNode("Camera");

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

    public override void _PhysicsProcess(float delta)
    {
        handleStrafing(delta);
    }
    private void handleStrafing(float delta)
    {

        Vector3 desiredMoveXY = new Vector3();
        Vector3 desiredMoveZ = new Vector3();
        //Add all the WASD controls to get a vector.
        if(inputEnabled)
        {
            if(Input.IsActionPressed("MoveForward"))
                desiredMoveXY += Vector3.Forward;
            if(Input.IsActionPressed("MoveLeft"))
                desiredMoveXY += Vector3.Left;
            if(Input.IsActionPressed("MoveBack"))
                desiredMoveXY += Vector3.Back;
            if(Input.IsActionPressed("MoveRight"))
                desiredMoveXY += Vector3.Right;
            if(Input.IsActionPressed("MoveUp"))
                desiredMoveZ += Vector3.Up;
            if(Input.IsActionPressed("MoveDown"))
                desiredMoveZ += Vector3.Down;
        }
        //what's the behavior of Normalized() when desiredMove is zero?
        //I guess it's still zero?
        var desiredMove = (desiredMoveXY + desiredMoveZ).Normalized()*maxSpeed;
        //desiredMove is still in local space.
        //We want to convert it to global space.
        Vector3 globalMove = LookPitch.GlobalTransform.basis.Xform(desiredMove);
        Translation += globalMove * delta;
        //We just apply movement manually since spectator cam doesn't have any interactions.
    }
}
