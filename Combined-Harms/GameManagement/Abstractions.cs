using Godot;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ReplicationAbstractions
{

    //Mixin for what used to be extension methods,
    //since extension methods can't be triggered by signals.
    public class ReplicationMember : Godot.Object
    {
        private IReplicable master;

        public ReplicationMember(IReplicable m)
        {
            master = m;
        }

        public void MasterDespawn()
        {
            ReplicationServer.Instance.Rpc(nameof(ReplicationServer.Despawn), ((Node) master).GetPath());
        }
        public void GenName()
        {
            master.Name = System.Text.Encoding.ASCII.GetString(Guid.NewGuid().ToByteArray());
        }

        public void OnPeerConnected(int uid)
        {
            if( master.IsNetworkMaster())
                ReplicationServer.Instance.ReplicateID(master, uid);
        }

        public void OnConnectedToSession(int uid)
        {
            MasterDespawn();
        }

        //When you want to have different behavior,
        //derive ReplicationMember and override this function.
        public virtual void OnNOKTransfer(int uid)
        {
            MasterDespawn();
        }
    }

    public interface IReplicable
    {
        ReplicationMember rMember {get;set;}
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
        
            n.rMember = new ReplicationMember(n);
            Networking.Instance.RTCMP.Connect("peer_connected", n.rMember,nameof(ReplicationMember.OnPeerConnected));
            Networking.Instance.Connect(nameof(Networking.ConnectedToSession), n.rMember, nameof(ReplicationMember.OnConnectedToSession));
            
            if( n.IsNetworkMaster())
            {
                n.rMember.GenName();
                ReplicationServer.Instance.Replicate(n);
            }
            else
            {
                NOKManager.Instance.Subscribe(n);
            }
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

