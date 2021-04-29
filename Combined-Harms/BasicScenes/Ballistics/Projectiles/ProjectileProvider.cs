using Godot;
using System;
using System.Collections.Generic;
using MessagePack;

using ReplicationAbstractions;
/*
Base class for small fast moving objects
that do something when they hit something else.

These never change masters.
If the master disappears, so does the projectile.
*/

public class ProjectileProvider : Node, IReplicable, IFPV, I3PV, IBufferedRPC
{

    //Replicable boilerplate
    public ReplicationMember rMember {get; set;}
    [Export]
    public string ScenePath {get;set;}
    //Observer boilerplate
    [Export]
    public string ObserverPathFPV {get;set;}
    [Export]
    public string ObserverPath3PV {get;set;}

    public Vector3 LastTranslation;
    public Vector3 LastLinearVelocity;


    [Signal]
    public delegate void TrajectoryUpdated( Vector3 translation, Vector3 velocity);

    [PuppetSync]
    public void Init(Vector3 translation, Vector3 velocity)
    {
        LastTranslation = translation;
        LastLinearVelocity = velocity;
        RigidBody observer = (RigidBody) EasyInstancer.GenObserver(this, (IsNetworkMaster()) ?  ObserverPathFPV: ObserverPath3PV);
        GetNode("/root/GameRoot/Map").AddChild(observer);
    }

    public override void _Ready()
    {
        this.ReplicableReady();
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class Vector3Packet
    {
        public float x;
        public float y;
        public float z;

        [IgnoreMember]
        public Vector3 vector
        {
            get{ return new Vector3(x,y,z); }
            set{
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public Vector3Packet(){}

    }

    [MessagePackObject]
    public class TrajectoryPacket
    {   
        [Key(0)]
        public Vector3Packet translation;
        [Key(1)]
        public Vector3Packet velocity;

        public TrajectoryPacket(){}

        public TrajectoryPacket(Vector3 translation, Vector3 velocity)
        {
            this.translation = new Vector3Packet { vector = translation };
            this.velocity = new Vector3Packet{ vector = velocity};
        }
    }

    public void UpdateTrajectory(Vector3 translation, Vector3 velocity)
    {
        
        TrajectoryPacket p = new TrajectoryPacket(translation, velocity);
        byte[] packet = MessagePackSerializer.Serialize<TrajectoryPacket>(p);
        BufRPCServer.Instance.Rpc(nameof(BufRPCServer.BufRPC), GetPath(), packet);
    }

    public virtual void HandlePacket( byte[] packet)
    {
        TrajectoryPacket p = MessagePackSerializer.Deserialize<TrajectoryPacket>(packet);
        LastTranslation = p.translation.vector;
        LastLinearVelocity = p.velocity.vector;
        EmitSignal(nameof(TrajectoryUpdated), LastTranslation, LastLinearVelocity);
    }
}
