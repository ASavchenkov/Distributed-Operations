using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleProvider : Node, IReplicable, IFPV
{

    //IReplicable boilerplate
    public ReplicationMember rMember {get; set;}
    
    [Export]
    public string ScenePath {get;set;}

    //IFPV boilerplate
    [Export]
    public string ObserverPathFPV {get; set;}

    Dictionary<string,Node> Attachments;
    //Maps name of attachment point to the actual attachment that is there.
    //Node is null if nothing is attached there.
    //Observers are responsible for properly inserting attachments
    //into their own node structures.

    public Magazine Mag;

    [Signal]
    public delegate void AttachmentUpdated(string attachPoint, Node attachment);

    public override void _Ready()
    {
        this.ReplicableReady();
        Mag = Magazine.Factory.Instance();
        SetAttachment("magwell", Mag);
    }

    public void SetAttachment(string attachPoint, Node attachment)
    {
        Attachments[attachPoint] = attachment;
        EmitSignal(nameof(AttachmentUpdated),attachPoint, attachment);
    }

    [RemoteSync]
    public void SetMaster(int uid)
    {
        SetNetworkMaster(uid);
    }

}
