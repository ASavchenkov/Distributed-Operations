using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;
public class PlayerCharacterProvider : Node, IReplicable, IFPV, I3PV
{

    public ReplicationMember rMember {get; set;}

    public static NodeFactory<PlayerCharacterProvider> Factory = 
        new NodeFactory<PlayerCharacterProvider> ("res://BasicScenes/Player/PlayerCharacter/PlayerCharacterProvider.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}


    [Export]
    public string ObserverPathFPV { get; set;}
    [Export]
    public string ObserverPath3PV { get; set;}
    //For generating observers

    [Signal]
    public delegate void TrajectoryUpdated(Vector3 translation, Vector3 yaw, Vector3 pitch, Vector3 velocity);

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

    [Export]
    public float HP = 100;
    [Export]
    public float Armor = 50;

    public override void _Ready()
    {
        this.ReplicableReady();
        var MapNode = GetNode("/root/GameRoot/Map");
        
        Node observer = EasyInstancer.GenObserver(this, IsNetworkMaster() ? ObserverPathFPV : ObserverPath3PV);
        MapNode.AddChild(observer);
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
    public void UpdateTrajectory(Vector3 translation, Vector3 yaw, Vector3 pitch, Vector3 velocity)
    {
        Translation = translation;
        YawRotation = yaw;
        PitchRotation = pitch;

        EmitSignal(nameof(TrajectoryUpdated), translation, yaw, pitch, velocity);
        // Body.Translation = translation;
        // LookYaw.Rotation = yaw;
        // LookPitch.Rotation = pitch;
    }

    [Master]
    public void HitRPC( float damage, float pen, string part)
    {
        GD.Print("Got hit from: ", part);
        var armorDamage = damage * (1-pen);
        Armor -= armorDamage;
        if(Armor < 0)
        {
            HP += Armor;
            Armor = 0;
        }
        HP -= (damage * pen);

        Rpc(nameof(UpdateHP), HP, Armor);
    }

    [Puppet]
    public void UpdateHP( float hp, float armor)
    {
        HP = hp;
        Armor = armor;
    }
}
