using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public class Replicator : Node
{

    public static Replicator Instance { get; private set;}

    HashSet<int> occupiedIDs = new HashSet<int>();
    Random rnd = new Random();

    public override void _Ready()
    {
        Instance = this;
    }

    private int GenUniqueID()
    {
        int candidate = rnd.Next(1,UInt16.MaxValue);

        //will almost certainly never happen
        //but in case it does, this guarantees a unique ID if one is available.
        while(occupiedIDs.Contains(candidate))
        {
            if(candidate==UInt16.MaxValue)
                candidate = 1;
            else
                candidate++;
        }
        return candidate;
    }

    public void Replicate(ReplicableNode n)
    {
        Rpc(n.GetParent().GetPath(), n.Name, n.ScenePath);
    }

    public void ReplicateID(ReplicableNode n, int uid)
    {
        RpcId(uid, nameof(Replicate), n.GetParent().GetPath(), n.Name, n.ScenePath);     
    }
    
    [Remote]
    public void Replicate(string parent, string name, string scenePath)
    {
        
        var parentNode = GetNode(parent);
        if(parentNode is null)
        {
            GD.Print("parent node path :", parent,"| invalid");
            return;
        }
        

        var childNode = (ReplicableNode) GetNode(parent + "/" + name);
        if(!(childNode is null))
        {
            //check if someone is simply confirming replication
            //Due to the loss of the ACK packet, or NOK transfer.
            if(childNode.ScenePath == scenePath 
                &&childNode.GetNetworkMaster() == GetTree().GetRpcSenderId())
            {
                childNode.Rpc(nameof(ReplicableNode.AckRPC));
            }
            else GD.Print("Replication Error: Node path collision");
            //purely for debugging. Shouldn't happen.
            //Don't know how to deal with this in production.
        }
        
        else
        {
            //If it doesn't exist yet, then just replicate it.
            //Easiest case to handle.
            PackedScene scene = GD.Load<PackedScene>(scenePath);
            childNode = (ReplicableNode) scene.Instance();
            parentNode.AddChild(childNode);

            childNode.Name = name;
            childNode.SetNetworkMaster(GetTree().GetRpcSenderId());
            childNode.Rpc(nameof(ReplicableNode.AckRPC));
            
        }
    
    }

}
