using Godot;
using System;

using ReplicationAbstractions;

//Specifically for controlling first person movement.
public class PlayerCharacterFPV : RigidBody, IObserver
{

    private PlayerCharacterProvider provider = null;

    public Spatial LookYaw;
    public Spatial LookPitch;
    public Camera camera;
    public Area FEET;

    [Export]
    float mouseSensitivity = 100;
    
    int groundCounter = 0;
    
    private RifleFPV ItemInHands = null;

    private InventoryMenu InvMenu;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LookYaw = (Spatial) GetNode("LookYaw");
        LookPitch = (Spatial) LookYaw.GetNode("LookPitch");
        camera = (Camera) LookPitch.GetNode("Camera");

        InvMenu = InventoryMenu.Factory.Instance();
        InvMenu.Subscribe(provider);
        
        FEET = (Area) GetNode("FEET");
        FEET.Connect("body_entered",this,"GroundEncountered");
        FEET.Connect("body_exited", this, "GroundLeft");

    }

    public void Subscribe(Node _provider)
    {
        provider = (PlayerCharacterProvider) _provider;
        provider.HandItem.Connect( nameof(LootSlot.OccupantSet), this, nameof(OnHandItemSet));
        this.DefaultSubscribe(provider);
    }

    public void OnHandItemSet(Node node)
    {
        GD.Print("setting hand item to rifle");
        if(IsInstanceValid(ItemInHands))
            ItemInHands.QueueFree();
        ItemInHands = (RifleFPV) EasyInstancer.GenObserver(node, ((IHasFPV)node).ObserverPathFPV);
        camera.AddChild(ItemInHands);

    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(inputEvent is InputEventMouseMotion mouseEvent)
        {
            //Yes these look flipped. It's correct.
            LookYaw.RotateY(-mouseEvent.Relative.x/mouseSensitivity);
            LookPitch.RotateX(-mouseEvent.Relative.y/mouseSensitivity);
            if(LookPitch.RotationDegrees.x > provider.maxPitch)
                LookPitch.RotationDegrees = new Vector3(provider.maxPitch,0,0);
            else if (LookPitch.RotationDegrees.x < -provider.maxPitch)
                LookPitch.RotationDegrees = new Vector3(-provider.maxPitch,0,0);
            GetTree().SetInputAsHandled();
        }
        else if (inputEvent is InputEventKey keyPress)
        {
            if(keyPress.IsActionPressed("Jump") && groundCounter!=0)
            {
                GD.Print(groundCounter);
                ApplyCentralImpulse(provider.jumpImpulse * Vector3.Up);    
                GetTree().SetInputAsHandled();
            }
            else if(keyPress.IsActionPressed("Inventory"))
            {
                if( InvMenu.IsInsideTree())
                    camera.RemoveChild(InvMenu);
                else
                    camera.AddChild(InvMenu);
                GetTree().SetInputAsHandled();
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        handleStrafing();
        provider.Rpc("UpdateTrajectory", Translation, LinearVelocity, LookYaw.Rotation, LookPitch.Rotation);
    }

    public void GroundEncountered(Node body)
    {
        groundCounter++;
    }

    public void GroundLeft(Node body)
    {   
        //In case it is possible for an object to enter but not ever leave.
        if(groundCounter>0) 
            groundCounter--;
    }

    private void handleStrafing()
    {

        Vector3 desiredMove = new Vector3();

        //Add all the WASD controls to get a vector.
        
        if(Input.IsActionPressed("MoveForward"))
            desiredMove += Vector3.Forward;
        if(Input.IsActionPressed("MoveLeft"))
            desiredMove += Vector3.Left;
        if(Input.IsActionPressed("MoveBack"))
            desiredMove += Vector3.Back;
        if(Input.IsActionPressed("MoveRight"))
            desiredMove += Vector3.Right;
        
        //what's the behavior of Normalized() when desiredMove is zero?
        //I guess it's still zero?
        desiredMove = desiredMove.Normalized()*provider.maxSpeed;
        //desiredMove is still in local space.
        //We want to convert it to global space.
        Vector3 globalMove = LookYaw.GlobalTransform.basis.Xform(desiredMove);
        Vector3 horizontalVelocity = LinearVelocity;
        horizontalVelocity.y = 0;
        
        //If we're not on the ground, reduce our control authority.
        float authority = groundCounter == 0 ? provider.acceleration/10 : provider.acceleration; 
        AddCentralForce((globalMove-horizontalVelocity)*authority);
    }

    public void TakeDamage(int damage, int penetration)
    {
        GD.Print("torso was hit");
    }

}
