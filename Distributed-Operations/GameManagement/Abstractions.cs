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
        public IReplicable owner;
        public bool ReplicateOnReady = false;

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

    //for things that can't just be deserialized and instanced
    //Such as nodes inside of scenes that are created automatically
    //but may still have some data that needs to be appplied.

    //Can be, and is, used in conjunction with IReplicable for
    //serializedNode. (but maybe not in the future?)
    public interface ISaveable
    {
        void ApplyData(object data);
        object GetData();
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
        public static void ReplicableReady( this IReplicable n, bool replicate = true)
        {
        
            n.rMember.owner = n;
            
            if(!n.IsNetworkMaster())
            {
                NOKManager.Instance.Subscribe(n);
            }
            else if(n.rMember.ReplicateOnReady)
            {
                n.rMember.GenName();
                if(replicate) n.Replicate();
            }
        }

        //Sometimes you want to replicate separately from startup
        public static void Replicate( this IReplicable n)
        {
            Networking.Instance.RTCMP.Connect("peer_connected", n.rMember,nameof(ReplicationMember.OnPeerConnected));
            ReplicationServer.Instance.Replicate(n);
        }
    }

    //Well golly this seems really vague!
    //Just like void* structs!
    //Cast to correct type inside implementation.
    //You can take the programmer out of the c++
    //but you can't take the c++ out of the programmer.
    public interface IObserver
    {
        void Subscribe(object _provider);
    }
    
    public static class EasyInstancer
    {
        public static int NetworkID = 1;
        public static T Instance<T> (string scenePath) where T: Node
        {
            PackedScene scene = GD.Load<PackedScene>(scenePath);
            Node n = scene.Instance();
            //This will get changed later by ReplicationServer if need be.
            n.SetNetworkMaster(NetworkID);
            return (T) n;
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

    public interface IHasInvPV
    {
        [Export]
        string ObserverPathInvPV { get; set;}
    }
}

