using Godot;
using System;
using System.Collections.Generic;

using MessagePack;

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
        return new SaveData(this);
    }


    //Currently pretty empty.
    //Going to have attachments and settings in the future.
    [MessagePackObject]
    public class SaveData : SerializedNode
    {
        [Key(3)]
        public int testInt;
        
        public SaveData(){}
        public SaveData(RifleProvider target) : base(target)
        {
            testInt = 8;
        }
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
