using Godot;
using System;


//Base Player class
//derived by LocalPlayer and RemotePlayer;
//has functionality common to both.
public class Player : Node
{
    
    public Spatial LookYaw;
    public Spatial LookPitch;
    public RigidBody Body;

    
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Body = (RigidBody) GetNode("Body");
        LookYaw = (Spatial) Body.GetNode("LookYaw");
        LookPitch = (Spatial) LookYaw.GetNode("LookPitch");

        //for now we're going to manually add the gun item
        //but in the future this happens outside the _Ready() function
        var gunScene = GD.Load<PackedScene>("res://BasicScenes/Items/Gun/Gun.tscn");
        var gunNode = gunScene.Instance();
        GetNode("Body/LookYaw/LookPitch").AddChild(gunNode);
    }

    [Remote]
    public void Hit()
    {
        GD.Print("Received Hit call");
    }
}
