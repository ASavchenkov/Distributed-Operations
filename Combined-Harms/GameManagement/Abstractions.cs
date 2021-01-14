using Godot;
using System;
using System.Collections.Generic;


//Valid casts are on the implementer,
//since there's no way to guarantee types at compile
//for scene instantiation.
public interface IObserver
{
    void Subscribe(Node provider)
    {
        //Prepend ID to make sure nothing collides.
        Name = provider.Name + "_" + Name;
        provider.Connect("tree_exiting", (Node) this, "queue_free");
    }
    #region NodeStuff
    string Name { get; set; }
    #endregion
}

//Create a static instance in your classes to make creation
//of nodes easier in code.
public class NodeFactory<T> where T: Node
{
    public string ScenePath {get; private set;}

    public NodeFactory(string scenePath)
    {
        ScenePath = scenePath;
    }

    public T Instance()
    {
        return EasyInstancer.Instance<T>(ScenePath);
    }   
}

public static class EasyInstancer
{
    public static T Instance<T> (string scenePath) where T: Node
    {
        PackedScene scene = GD.Load<PackedScene>(scenePath);
        return (T) scene.Instance();
    }
    public static Node GenObserver( Node provider, string path)
    {
        var observer = (IObserver) EasyInstancer.Instance<Node>(path);
        //somehow we need to know the type for Subscribe.
        observer.Subscribe(provider);
        return  (Node) observer;
    }
}

public interface IFPV
{
    [Export]
    string ObserverPathFPV { get; set;}
}

public interface I3PV
{
    [Export]
    string ObserverPath3PV { get; set;}
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

    [PuppetSync]
    void Despawn()
    {
        QueueFree();
    }
    
    void OnNOKTransfer(int uid)
    {
        Rpc(nameof(Despawn));
    }

    //Call with "((IReplicable) this).ready();" in _Ready()
    //How do we get rid of this boilerplate?
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