using Godot;
using System;
using System.Collections.Generic;

public class RifleProvider : Node, IProvider
{

    public event NotifyProviderEnd ProviderEnd;

    [Export]
    Dictionary<string,Node> Attachments;
    //Maps name of attachment point to the actual attachment that is there.
    //Node is null if nothing is attached there.
    //Observers are responsible for properly inserting attachments
    //into their own node structures.

    [Export]
    public Dictionary<string,string> ObserverPaths;
    //For generating observers

    public delegate void AttachmentUpdateHandler(string attachPoint, IProvider attachment);
    public event AttachmentUpdateHandler AttachmentUpdated;


    public Node GenerateObserver(string name)
    {
        var observer = (IObserver<RifleProvider>) GD.Load<PackedScene>(ObserverPaths[name]).Instance();
        observer.Init(this);
        return (Node) observer;
    }

    public void SetAttachment(string attachPoint, IProvider attachment)
    {
        Attachments[attachPoint] = (Node) attachment;
        AttachmentUpdated?.Invoke(attachPoint, attachment);
    }

    [RemoteSync]
    public void SetMaster(int uid)
    {
        SetNetworkMaster(uid);
    }

    ~RifleProvider()
    {
        ProviderEnd?.Invoke();
    }
    
}
