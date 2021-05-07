using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleProvider : Node, IReplicable, IFPV, ILootPV
{

    //IReplicable boilerplate
    public ReplicationMember rMember {get; set;}
    
    [Export]
    public string ScenePath {get;set;}

    [Export]
    public string ObserverPathFPV {get; set;}
    [Export]
    public string ObserverPathLootPV {get; set;}

    public Magazine Mag;

    
    public override void _Ready()
    {
        this.ReplicableReady();
        Mag = Magazine.Factory.Instance();
        AddChild(Mag);
    }

    [RemoteSync]
    public void SetMaster(int uid)
    {
        SetNetworkMaster(uid);
    }

}
