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
    string ScenePath {get;}
    HashSet<int> Unconfirmed {get;set;}

    [Remote]
    void AckRPC(int uid)
    {
        Unconfirmed.Remove(uid);    
    }
    
    void OnNOKTransfer(int uid);
    
    void ready()
    {
        Unconfirmed = new HashSet<int>(Networking.Instance.SignaledPeers.Keys);
        Networking.Instance.RTCMP.Connect("peer_connected", (Node) this,nameof(OnPeerConnected));
        Networking.Instance.RTCMP.Connect("peer_disconnected", (Node) this,nameof(OnPeerDC));
        Networking.Instance.Connect(nameof(Networking.ConnectedToSession), (Node) this, nameof(OnConnectedToSession));
        if(IsNetworkMaster())
        {
            GenName();
            ReplicationServer.Instance.Replicate(this);
        }
        else
        {
            NOKManager.Instance.Subscribe(this);
        }
    }
    
    private void GenName()
    {
        Name = System.Text.Encoding.ASCII.GetString(Guid.NewGuid().ToByteArray());
    }
    public void OnPeerConnected( int uid)
    {
        Unconfirmed.Add(uid);
    }

    void OnPeerDC(int uid)
    {
        Unconfirmed.Remove(uid);
    }

    void OnConnectedToSession(int uid)
    {
        QueueFree();
    }

    #region NODE_STUFF
    //This stuff is already implemented in Node
    //So to make sure we can use them on IReplicable types
    //we define them here;
    Node GetParent();
    string Name{get;set;}
    int GetNetworkMaster();
    bool IsNetworkMaster();
    void SetNetworkMaster(int uid, bool recursive = true);
    void QueueFree();
    object Rpc(string method, params object[] args);
    #endregion

}