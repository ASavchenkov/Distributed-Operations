using Godot;
using System;
using System.Collections.Generic;

public class PlayerCharacterProvider : Node, IProvider
{

    [Export]
    public Dictionary<string,string> ObserverPaths;
    //For generating observers

    [Signal]
    public delegate void TrajectoryUpdated(Vector3 translation, Vector3 yaw, Vector3 pitch);
    
    Vector3 Translation = new Vector3();
    Vector3 YawRotation = new Vector3();
    Vector3 PitchRotation = new Vector3();

    [Export]
    public float maxSpeed {get; private set;} = 10;
    [Export]
    public float acceleration {get; private set;} = 10;
    [Export]
    public float jumpImpulse {get; private set;} = 10;
    [Export]
    public float maxPitch {get; private set;} = 80; //degrees

    public override void _Ready()
    {
        
        var MapNode = GetNode("/root/GameRoot/Map");

        //If we're not the network master,
        //use the simplified observer.
        var observer = GenerateObserver(IsNetworkMaster() ? "FPV" : "3PV");
        MapNode.AddChild(observer);
    }

    public Node GenerateObserver(string name)
    {
        var observer = (IObserver<PlayerCharacterProvider>) GD.Load<PackedScene>(ObserverPaths[name]).Instance();
        observer.Init(this);
        return (Node) observer;
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
