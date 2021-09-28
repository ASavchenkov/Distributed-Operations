using Godot;
using System;

using MessagePack;

using ReplicationAbstractions;
using System.Diagnostics;

//Specifically for controlling first person movement.
public class PlayerCharacterFPV : RigidBody, ITakesInput, IObserver
{
    private bool _disposed = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    public PlayerCharacterProvider provider {get; private set;} = null;

    public Spatial LookYaw;
    public Spatial LookPitch;
    public Camera camera;
    public Area FEET;

    [Export]
    float mouseSensitivity = 100;
    
    int groundCounter = 0;
    
    private RifleFPV ItemInHands = null;

    // private InventoryMenu inventoryMenu;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LookYaw = GetNode<Spatial>("LookYaw");
        LookPitch = LookYaw.GetNode<Spatial>("LookPitch");
        camera = LookPitch.GetNode<Camera>("Camera");

        // inventoryMenu = EasyInstancer.Instance<InventoryMenu>("res://BasicScenes/GUI/2.5D UI/InventoryMenu.tscn");
        // inventoryMenu.pcFPV = this;
        
        FEET = (Area) GetNode("FEET");
        FEET.Connect("body_entered",this,"GroundEncountered");
        FEET.Connect("body_exited", this, "GroundLeft");

        Claims.Claims.UnionWith(InputPriorityServer.Instance.movementActions);
        InputPriorityServer.Base.Subscribe(this, BaseRouter.character);
    }

    public void Subscribe(object _provider)
    {
        provider = (PlayerCharacterProvider) _provider;
        provider.HandSlot.Connect( nameof(InvSlot.OccupantSet), this, nameof(OnHandItemSet));
        this.DefaultSubscribe(provider);
    }

    public void OnHandItemSet(Node node)
    {
        GD.Print("setting hand item to rifle");
        if(IsInstanceValid(ItemInHands))
            ItemInHands.QueueFree();
        ItemInHands = null;
        
        if(!(node is null))
        {
            ItemInHands = (RifleFPV) EasyInstancer.GenObserver(node, ((IHasFPV)node).ObserverPathFPV);
            camera.AddChild(ItemInHands);
        }
    }

    public bool OnInput(InputEvent inputEvent)
    {
        //If the equipped item handled the input, return
        //Otherwise keep going.
        if(!(ItemInHands is null) && ItemInHands.OnInput(inputEvent))
            return true;
        if(inputEvent is InputEventMouseMotion mouseEvent)
        {
            //Yes these look flipped. It's correct.
            LookYaw.RotateY(-mouseEvent.Relative.x/mouseSensitivity);
            LookPitch.RotateX(-mouseEvent.Relative.y/mouseSensitivity);
            if(LookPitch.RotationDegrees.x > provider.maxPitch)
                LookPitch.RotationDegrees = new Vector3(provider.maxPitch,0,0);
            else if (LookPitch.RotationDegrees.x < -provider.maxPitch)
                LookPitch.RotationDegrees = new Vector3(-provider.maxPitch,0,0);
            return true;
        }
        else if (inputEvent is InputEventKey keyPress)
        {
            if(keyPress.IsActionPressed("Jump") && groundCounter!=0)
            {
                GD.Print(groundCounter);
                ApplyCentralImpulse(provider.jumpImpulse * Vector3.Up);    
                return true;
            }
            
            else if(keyPress.IsActionPressed("ui_home"))
            {
                var readyObject = new SerializedNode(provider);
                GD.Print(readyObject.AsJson());
            }
        }
        return false;
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
        
        if(Claims.IsActionPressed("MoveForward"))
            desiredMove += Vector3.Forward;
        if(Claims.IsActionPressed("MoveLeft"))
            desiredMove += Vector3.Left;
        if(Claims.IsActionPressed("MoveBack"))
            desiredMove += Vector3.Back;
        if(Claims.IsActionPressed("MoveRight"))
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
