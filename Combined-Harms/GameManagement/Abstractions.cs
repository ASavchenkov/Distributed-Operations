using Godot;
using System;
using System.Collections.Generic;
public interface IProvider
{
    Node GenerateObserver(string name);
}

public interface IObserver<T> where T: IProvider
{
    void Init(T provider);
}

//Create a static instance in your classes to make creation
public class NodeFactory<T> where T: Node
{
    public string ScenePath {get; private set;}

    public NodeFactory(string scenePath)
    {
        ScenePath = scenePath;
    }

    public T Instance()
    {
        PackedScene scene = GD.Load<PackedScene>(ScenePath);
        return (T) scene.Instance();
    }   
}

public interface IReplicable
{
    Replicator memberReplicator {get;set;}
    void OnNOKTransfer(int uid);
    string ScenePath {get;}
    [Remote]
    void AckRPC(int uid);
    #region NODE_STUFF
    //This stuff is already implemented in Node
    //So to make sure we can use them on IReplicable types
    //we define them here;
    Node GetParent();
    string Name{get;set;}
    int GetNetworkMaster();
    bool IsNetworkMaster();
    void SetNetworkMaster(int uid, bool recursive = true);
    object Rpc(string method, params object[] args);
    #endregion

}

public class Replicator : Godot.Object
{
    IReplicable node;
    HashSet<int> Unconfirmed = new HashSet<int>();
    public Replicator(IReplicable n)
    {
        node = n;
        Networking.Instance.RTCMP.Connect("peer_connected", (Node) node,nameof(OnPeerConnected));
        Networking.Instance.RTCMP.Connect("peer_disconnected", (Node) node,nameof(OnPeerDC));

        if(node.IsNetworkMaster())
        {
            GenName();
            ReplicationServer.Instance.Replicate(node);
        }
        else
        {
            NOKManager.Instance.Subscribe(node);
        }
    }

    private void GenName()
    {
        node.Name = System.Text.Encoding.ASCII.GetString(Guid.NewGuid().ToByteArray());
    }
    public void OnPeerConnected( int uid)
    {
        Unconfirmed.Add(uid);
    }

    public void OnPeerDC(int uid)
    {
        Unconfirmed.Remove(uid);
    }

    //Called by ReplicationServer only.
    //Since we don't want to define the same rpc
    //for every single IReplicable Node
    public void AckRPC(int uid)
    {
        Unconfirmed.Remove(uid);
    }

}