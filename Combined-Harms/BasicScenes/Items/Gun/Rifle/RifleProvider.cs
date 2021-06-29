using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleProvider : Node, IReplicable, IHasFPV, ILootItem
{

    //IReplicable boilerplate
    public ReplicationMember rMember {get; set;}
    
    [Export]
    public string ScenePath {get;set;}

    [Export]
    public string ObserverPathFPV {get; set;}
    [Export]
    public string ObserverPathLootPV {get; set;}


    public LootSlot parent {get;set;} = null;

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

    public bool Validate(ILootItem item, object stateUpdate)
    {
        if(item == this)
            return false;
        return true;
    }

}
