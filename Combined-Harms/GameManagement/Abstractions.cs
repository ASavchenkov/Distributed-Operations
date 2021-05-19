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
        private IReplicable owner;

        public ReplicationMember(IReplicable o)
        {
            owner = o;
        }

        public void MasterDespawn()
        {
            ReplicationServer.Instance.Rpc(nameof(ReplicationServer.Despawn), ((Node) owner).GetPath());
        }
        public void GenName()
        {
            owner.Name = Guid.NewGuid().ToString("D");
        }

        public void OnPeerConnected(int uid)
        {
            if( IsInstanceValid((Node) owner) && owner.IsNetworkMaster())
                ReplicationServer.Instance.ReplicateID(owner, uid);
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


    //Actual observer implementation 
    public interface IHasFPV
    {
        [Export]
        string ObserverPathFPV { get; set;}
    }

    public interface IHas3PV
    {
        [Export]
        string ObserverPath3PV { get; set;}
    }

    public interface IHasLootPV
    {
        [Export]
        string ObserverPathLootPV { get; set;}
    }
}

