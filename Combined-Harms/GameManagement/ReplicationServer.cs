using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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
    
    [Remote]
    public void Replicate(string parent, string name, string scenePath)
    {
        
        var parentNode = GetNode(parent);
        if(parentNode is null)
        {
            GD.Print("parent node path :<", parent,"> invalid");
            return;
        }
        

        var childNode = (IReplicable) GetNode(parent + "/" + name);
        if(!(childNode is null))
        {
            //check if someone is simply confirming replication
            //Due to the loss of the ACK packet, or NOK transfer.
            if(childNode.ScenePath == scenePath 
                &&childNode.GetNetworkMaster() == GetTree().GetRpcSenderId())
            {
                childNode.Rpc(nameof(IReplicable.AckRPC));
            }
            else GD.Print("Replication Error: Node path collision/ non-master call");
            //purely for debugging. Shouldn't happen.
            //Don't know how to deal with this in production.
        }
        else
        {
            //If it doesn't exist yet, then just replicate it.
            //Easiest case to handle.
            PackedScene scene = GD.Load<PackedScene>(scenePath);
            childNode = (IReplicable) scene.Instance();
            parentNode.AddChild((Node) childNode);

            childNode.Name = name;
            childNode.SetNetworkMaster(GetTree().GetRpcSenderId());
            childNode.Rpc(nameof(IReplicable.AckRPC));   
        }
    
    }

}
