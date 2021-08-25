using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleProvider : Node, IReplicable, IHasFPV, IInvItem
{

    //IReplicable boilerplate
    public ReplicationMember rMember {get; set;}
    
    [Export]
    public string ScenePath {get;set;}

    [Export]
    public string ObserverPathFPV {get; set;}
    [Export]
    public string ObserverPathInvPV {get; set;}


    public InvSlot parent {get;set;} = null;

    public Magazine Mag;
    

    public SerializedNode Serialize()
    {
        return new SerializedNode(this);
    }


    //Currently pretty empty.
    //Going to have attachments and settings in the future.
    public class SaveData : SerializedNode
    {
        public SaveData(RifleProvider target) : base(target){}
    }

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

    public bool Validate(IInvItem item, object stateUpdate)
    {
        if(item == this)
            return false;
        return true;
    }

}
