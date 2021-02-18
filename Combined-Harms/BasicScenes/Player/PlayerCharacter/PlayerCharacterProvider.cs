using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;
public class PlayerCharacterProvider : Node, IReplicable, IFPV, I3PV
{

    public static NodeFactory<PlayerCharacterProvider> Factory = 
        new NodeFactory<PlayerCharacterProvider> ("res://BasicScenes/Player/PlayerCharacter/PlayerCharacterProvider.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}
    public HashSet<int> Unconfirmed {get; set;}

    [Export]
    public string ObserverPathFPV { get; set;}
    [Export]
    public string ObserverPath3PV { get; set;}
    //For generating observers

    [Signal]
    public delegate void TrajectoryUpdated(Vector3 translation, Vector3 yaw, Vector3 pitch);

    Vector3 Translation = new Vector3();
    Vector3 YawRotation = new Vector3();
    Vector3 PitchRotation = new Vector3();

    [Signal]
    public delegate void HandItemUpdated(string nodePath);

    [Export]
    public float maxSpeed {get; private set;} = 10;
    [Export]
    public float acceleration {get; private set;} = 10;
    [Export]
    public float jumpImpulse {get; private set;} = 10;
    [Export]
    public float maxPitch {get; private set;} = 80; //degrees
    [Export]
    public string GunPath{get; private set;}

    RifleProvider ItemInHands = null;

    public override void _Ready()
    {
        this.ReplicableReady();
        
        var MapNode = GetNode("/root/GameRoot/Map");
        //If we're not the network master,
        //use the simplified observer.
        Node observer = EasyInstancer.GenObserver(this, IsNetworkMaster() ? ObserverPathFPV : ObserverPath3PV);
        MapNode.AddChild(observer);

        RifleProvider M4A1 = (RifleProvider) GD.Load<PackedScene>(GunPath).Instance();
        GetNode("/root/GameRoot/Loot").AddChild(M4A1);
        SetHandItem(M4A1);

        if(!IsNetworkMaster())
        {
            NOKManager.Instance.Subscribe(this);
        }
        
    }

    public void OnNOKTransfer(int uid)
    {
        QueueFree();
    }

    public void SetHandItem(RifleProvider item)
    {
        ItemInHands = item;
        EmitSignal(nameof(HandItemUpdated),ItemInHands);
    }

    [PuppetSync]
    public void UpdateTrajectory(Vector3 translation, Vector3 yaw, Vector3 pitch)
    {
        Translation = translation;
        YawRotation = yaw;
        PitchRotation = pitch;

        EmitSignal(nameof(TrajectoryUpdated), translation, yaw, pitch);
        // Body.Translation = translation;
        // LookYaw.Rotation = yaw;
        // LookPitch.Rotation = pitch;
    }
}
