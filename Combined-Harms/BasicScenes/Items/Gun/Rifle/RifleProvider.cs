using Godot;
using System;
using System.Collections.Generic;

public class RifleProvider : Node, IProvider
{

    
    [Export]
    Dictionary<string,Node> Attachments;
    //Maps name of attachment point to the actual attachment that is there.
    //Node is null if nothing is attached there.
    //Observers are responsible for properly inserting attachments
    //into their own node structures.

    [Export]
    public Dictionary<string,string> ObserverPaths;
    //For generating observers

    [Signal]
    public delegate void AttachmentUpdated(string attachPoint, Node attachment);

    public Node GenerateObserver(string name)
    {
        var observer = (IObserver<RifleProvider>) GD.Load<PackedScene>(ObserverPaths[name]).Instance();
        observer.Init(this);
        return (Node) observer;
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
