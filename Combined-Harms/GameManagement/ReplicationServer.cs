using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using ReplicationAbstractions;

public class ReplicationServer : Node
{

    public static ReplicationServer Instance { get; private set;}

    public override void _Ready()
    {
        Instance = this;
    }

    public void Replicate(IReplicable n)
    {
        Rpc(nameof(Replicate), n.GetParent().GetPath(), n.Name, n.ScenePath);
    }

    public void ReplicateID(IReplicable n, int uid)
    {
        RpcId(uid, nameof(Replicate), n.GetParent().GetPath(), n.Name, n.ScenePath);     
    }

    //No checking here yet
    [RemoteSync]
    public void Despawn(string path)
    {
        GD.Print("Despawning: ", path);
        GetNode(path).QueueFree();
    }
    
    [Remote]
    public void Replicate(string parent, string name, string scenePath)
    {
        
        var parentNode = GetNode(parent);
        if(parentNode is null)
        {
            GD.Print("parent node path :<", parent,"> invalid");
            return;
        }
        
        string childPath = parent+ "/" + name;
        var childNode = (IReplicable) GetNode(childPath);
        if(childNode is null)
        {
            //If it doesn't exist yet, then just replicate it.
            //Easiest case to handle.
            PackedScene scene = GD.Load<PackedScene>(scenePath);
            childNode = (IReplicable) scene.Instance();
            parentNode.AddChild((Node) childNode);

            childNode.Name = name;
            childNode.SetNetworkMaster(GetTree().GetRpcSenderId());
        }
        else
        {
            GD.PrintErr("Replication Error: Node path collision/ non-master call");
            //purely for debugging. Shouldn't happen.
            //Don't know how to deal with this in production.
        }
    
    }

}
