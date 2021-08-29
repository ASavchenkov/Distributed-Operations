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
        Rpc(nameof(ReplicateRPC), n.GetParent().GetPath(), n.Name, n.ScenePath);
    }

    public void ReplicateID(IReplicable n, int uid)
    {
        RpcId(uid, nameof(ReplicateRPC), n.GetParent().GetPath(), n.Name, n.ScenePath);     
    }

    //Not necessary to call this for every object.
    //Most projectiles, for example, will have their own function for despawning.
    //That way they don't get deleted before they can make their visual effects.
    [RemoteSync]
    public void Despawn(string path)
    {
        GD.Print("Despawning: ", path);
        GetNode(path).QueueFree();
    
    }
    
    [Remote]
    public void ReplicateRPC(string parent, string name, string scenePath)
    {
        GD.Print("Replicating: ", parent, "; ", name, "; ", scenePath);
        GD.Print("Peer ID: ", GetTree().GetRpcSenderId());
        
        var parentNode = GetNode(parent);
        if(parentNode is null)
        {
            GD.Print("parent node path :<", parent,"> invalid");
            return;
        }
        
        string childPath = parent+ "/" + name;
        var childNode = (IReplicable) GetNodeOrNull(childPath);
        if(childNode is null)
        {
            //If it doesn't exist yet, then just replicate it.
            //Easiest case to handle.
            childNode = (IReplicable) EasyInstancer.Instance<Node>(scenePath);

            childNode.Name = name;
            var sender = GetTree().GetRpcSenderId();
            childNode.SetNetworkMaster(sender);
            parentNode.AddChild((Node) childNode);
            if(listeners.ContainsKey(childPath))
            {
                foreach(NotifyReplicated listener in listeners[childPath])
                    listener((Node) childNode);
                listeners.Remove(childPath);
            }

        }
        else
        {
            GD.PrintErr("Replication Error: Node path collision/ non-master call");
            GD.PrintErr(childPath);
            //purely for debugging. Shouldn't happen.
            //Don't know how to deal with this in production.
        }
    }

    public delegate void NotifyReplicated(Node n);
    Dictionary<NodePath, HashSet<NotifyReplicated>> listeners = new Dictionary<NodePath, HashSet<NotifyReplicated>>();
    
    public void Subscribe(NodePath path, NotifyReplicated listener)
    {
        HashSet<NotifyReplicated> l;
        listeners.TryGetValue(path, out l);
        l.Add(listener);
    }
}

#region BadCode
//ref that listens to Replication server for replication of the node it's looking for.
//When the referenced node is replicated, it will start returning the node instead of null
//Primarily used when replicating. We send the node path for references.
// public class ReplicableRef<T> : Godot.Object, IEquatable<ReplicableRef<T>> where T : class, IReplicable
// {
//     string listenedPath;
//     //Make a new ReplicableRef instead of setting Value.
//     //Treat this basically as a struct.
//     public T Value {get; private set;}

//     public ReplicableRef( NodePath n)
//     {
//         Value = (T) (IReplicable) ReplicationServer.Instance.GetNodeOrNull(n);
//         if(Value == null)
//         {
//             listenedPath = n;
//             ReplicationServer.Instance.Connect(nameof(ReplicationServer.NodeAdded), this, nameof(OnReplicatedNode));
//         }
//     }

//     //Sometimes we're referencing things locally,
//     //In which case construct using T.
//     public ReplicableRef(T input)
//     {
//         Value = input;
//     }

//     public void OnReplicatedNode(Node n)
//     {
//         if(n.GetPath() == listenedPath)
//         {
//             Value = (T) (IReplicable) n; //It's always gonna be IReplicable
//             ReplicationServer.Instance.Disconnect(nameof(ReplicationServer.NodeAdded), this, nameof(OnReplicatedNode));
//         }
//     }

//     //Still want to be able to treat it as a T for purpose of convenience
//     public static implicit operator T(ReplicableRef<T> r)
//     {
//         return r?.Value;
//     }
    
//     // override object.Equals
//     public override bool Equals(object obj)
//     {
        
//         if (obj == null || GetType() != obj.GetType())
//             return false;
        
//         return Equals(obj as ReplicableRef<T>);
//     }
//     public bool Equals(ReplicableRef<T> other)
//     {   
//         //Either they're listening for the same node,
//         if(Value is null) return listenedPath == other.listenedPath;
//         //Or they refer to the same node.
//         else return Value == other.Value;
//         //Two equal ReplicableRefs always fall into one of these two situations.
//     }
    
//     // override object.GetHashCode
//     public override int GetHashCode()
//     {
//         //Hash Codes always based on paths
//         //That way a ReplicableRef maps to the same hash code
//         //even after it has been "fulfilled."
//         if(Value is null) return listenedPath.GetHashCode();
//         else return Value.GetPath().GetHashCode();
//     }
// }
#endregion