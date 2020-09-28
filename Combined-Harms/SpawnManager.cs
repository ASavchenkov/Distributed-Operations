using Godot;
using System;
using System.Collections.Generic;
//Handles the naming of Nodes spawned by the local peer.
//And the signaling necessary to replicate these nodes on remote peers.
//As well as other nodes that we may have become master of in the course of action
public class SpawnManager : Node
{

    //Makeshift bidirectional dictionary for insertion and deletion from tree.
    public Dictionary<int,Node> forwardMap = new Dictionary<int, Node>();
    Dictionary<int,string> forwardSceneMap = new Dictionary<int,string>();
    Dictionary<Node,int> backwardMap = new Dictionary<Node, int>();

    Random rnd = new Random();

    [Signal]
    public delegate void Cleared();

    public override void _Ready()
    {
        GetTree().Connect("network_peer_connected",this, "OnPeerConnected");
        GetNode("/root/GameRoot/Networking").Connect("ConnectedToSession",this, "ClearAll");
    }

    //Copied but slightly different from what's in HandshakeServer
    //Don't want to have that weird dependency
    private int GenUniqueID()
    {
        int candidate = rnd.Next(1,UInt16.MaxValue);

        //will almost certainly never happen
        //but in case it does, this guarantees a unique ID if one is available.
        while(forwardMap.ContainsKey(candidate))
        {
            if(candidate==UInt16.MaxValue)
                candidate = 1;
            else
                candidate++;
        }
        return candidate;
    }

    //When we join someone else's game,
    //clear everything and start from scratch.
    //Since we can't send the whole object in it's current state,
    //we clear everything and have the game developer handle
    //re-initialization, since it's likely it will need to happen anyways.
    public void ClearAll(int uid)
    {
        GD.Print("ClearAll");
        var children = GetChildren();
        foreach( Node child in children)
            child.QueueFree();

        EmitSignal("Cleared");
    }
    
    private Node Insert(string scenePath, int spawnerUID, int nodeID)
    {
        //We first load the scene and instance the node as we normally would
        PackedScene scene = GD.Load<PackedScene>(scenePath);
        Node instance = scene.Instance();
        
        //Name with a hex ID.
        string name = spawnerUID.ToString("X4") + "_" + nodeID.ToString("X4");
        instance.Name = name;
        instance.SetNetworkMaster(spawnerUID);
        AddChild(instance);
        
        return instance; 
    }

    [Remote]
    public void Replicate(string scenePath, int spawnerUID, int nodeID)
    {
        Insert(scenePath, spawnerUID, nodeID);
    }

    private void PrintDictionary(Dictionary<int, Node> dict)
    {
        foreach( KeyValuePair<int,Node> kvp in dict)
        {
            GD.Print("Key: ", kvp.Key);
        }
    }

    public void Remove(int nodeID)
    {
        Node node = forwardMap[nodeID];
        forwardMap.Remove(nodeID);
        forwardSceneMap.Remove(nodeID);
        backwardMap.Remove(node);
    }

    public Node Spawn(string scenePath)
    {
        int spawnerUID = GetTree().GetNetworkUniqueId();
        int nodeID = GenUniqueID();
        
        //Create the node both locally and remotely
        Node instance = Insert(scenePath, spawnerUID, nodeID);
        Rpc("Replicate",scenePath, spawnerUID, nodeID);

        //then add it to our dictionaries since we're the ones spawning
        forwardMap.Add(nodeID, instance);
        forwardSceneMap.Add(nodeID, scenePath);
        backwardMap.Add(instance, nodeID);
        instance.Connect("tree_exiting", this, "Remove",
            new Godot.Collections.Array(new int[] {nodeID}));
        return instance;

    }

    //When we connect to someone else (or they connect to us)
    //tell them what they need to replicate to understand the RPCs
    //the children of this node are calling.
    public void OnPeerConnected(int uid)
    {
        GD.Print("OnPeerConnected");
        int thisUID = GetTree().GetNetworkUniqueId();
        foreach(KeyValuePair<int, string> kvp in forwardSceneMap)
        {
            RpcId(uid, "Replicate", kvp.Value, thisUID, kvp.Key);
        }
    }

}
