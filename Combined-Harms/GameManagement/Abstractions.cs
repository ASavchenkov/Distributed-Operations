using Godot;
using System;
using System.Collections.Generic;

namespace ReplicationAbstractions
{

    public interface IReplicable
    {

        HashSet<int> Unconfirmed {get;set;}
        string ScenePath {get;}

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
        NodePath GetPath();
        #endregion
    }
    
    public static class ReplicationExtensions
    {
        public static void ReplicableReady( this IReplicable n)
        {
        
            n.Unconfirmed = new HashSet<int>(Networking.Instance.SignaledPeers.Keys);
            Networking.Instance.RTCMP.Connect("peer_connected", (Node) n,nameof(OnPeerConnected));
            Networking.Instance.RTCMP.Connect("peer_disconnected", (Node) n,nameof(OnPeerDC));
            Networking.Instance.Connect(nameof(Networking.ConnectedToSession), (Node) n, nameof(OnConnectedToSession));
            
            if( n.IsNetworkMaster())
            {
                n.GenName();
                ReplicationServer.Instance.Replicate(n);
            }
            else
            {
                NOKManager.Instance.Subscribe(n);
            }
        }
        public static void Ack(this IReplicable n, int uid)
        {
            n.Unconfirmed.Remove(uid);
        }
        public static void MasterDespawn(this IReplicable n)
        {
            ReplicationServer.Instance.Rpc(nameof(ReplicationServer.Despawn), ((Node)n).GetPath());
        }
        public static void GenName(this IReplicable n)
        {
            n.Name = System.Text.Encoding.ASCII.GetString(Guid.NewGuid().ToByteArray());
        }

        public static void OnNOKTransfer(this IReplicable n, int uid)
        {
            n.MasterDespawn();
        }
        public static void OnPeerConnected( this IReplicable n, int uid)
        {
            n.Unconfirmed.Add(uid);
        }

        public static void OnPeerDC( this IReplicable n, int uid)
        {
            n.Unconfirmed.Remove(uid);
        }

        public static void OnConnectedToSession( this IReplicable n, int uid)
        {
            n.QueueFree();
        }
    }
    public interface IObserver
    {
        void Subscribe(Node provider);
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
            Node observer = Instance<Node>(path);
            if(observer is IObserver o)
            {
                o.Subscribe(provider);
            }else{
                observer.DefaultSubscribe(provider);
            }
            return  (Node) observer;
        }
        //Default subscription function.
        //Implement one internally yourself when you want something else.
        public static void DefaultSubscribe( this Node observer, Node provider)
        {
            observer.Name = provider.Name + "_" + observer.Name;
            provider.Connect("tree_exiting", observer, "queue_free");
        }
    }

    //Create a static instance in your classes to make creation
    //of nodes easier in code (optional)
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
}

