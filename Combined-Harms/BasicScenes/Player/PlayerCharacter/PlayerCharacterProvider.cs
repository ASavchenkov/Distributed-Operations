using Godot;
using System;
using System.Collections.Generic;

public class PlayerCharacterProvider : Node, IProvider, IReplicable
{

    public static NodeFactory<PlayerCharacterProvider> Factory = 
        new NodeFactory<PlayerCharacterProvider> ("res://BasicScenes/Player/PlayerCharacter/PlayerCharacterProvider.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}
    public Replicator memberReplicator {get;set;}
    
    [Remote]
    public void AckRPC(int uid)
    {
        memberReplicator.AckRPC(uid);
    }

    [Export]
    public Dictionary<string,string> ObserverPaths;
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

    RifleProvider ItemInHands = null;

    public override void _Ready()
    {
        memberReplicator = new Replicator(this);
        
        var MapNode = GetNode("/root/GameRoot/Map");
        //If we're not the network master,
        //use the simplified observer.
        var observer = GenerateObserver(IsNetworkMaster() ? "FPV" : "3PV");
        MapNode.AddChild(observer);

        var lootSpawner = (SpawnManager) GetNode("/root/GameRoot/Loot");
        SetHandItem ((RifleProvider) lootSpawner.Spawn("res://BasicScenes/Items/Gun/Rifle/M4A1/M4A1Provider.tscn"));

        if(!IsNetworkMaster())
        {
            NOKManager.Instance.Subscribe(this);
        }
        
    }

    public void OnNOKTransfer(int uid)
    {
        QueueFree();
    }

    public Node GenerateObserver(string name)
    {
        var observer = (IObserver<PlayerCharacterProvider>) GD.Load<PackedScene>(ObserverPaths[name]).Instance();
        observer.Init(this);
        return (Node) observer;
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
