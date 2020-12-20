using Godot;
using System;
using System.Collections.Generic;

//Primarily meant to be extended as Godot.Node is
//But can also simply be dragged and dropped on root nodes of scenes
//(as long as they have no other functionality)
//Don't forget to input the ScenePath string when not using NodeFactory

public interface Test
{
    public void lol()
    {
        
    }
}
public class ReplicableNode : Node
{
    
    private HashSet<int> unconfirmed;
    
    [Export]
    public virtual string ScenePath{get;set;}

    //Overriding _Ready functions need to first call base._Ready.
    public override void _Ready()
    {
        unconfirmed = new HashSet<int>(Networking.Instance.SignaledPeers.Keys);
        Networking.Instance.RTCMP.Connect("peer_connected",this,nameof(OnPeerConnected));
        Networking.Instance.RTCMP.Connect("peer_disconnected",this,nameof(OnPeerDC));

        if(IsNetworkMaster())
        {
            SetName();
            Replicator.Instance.Replicate(this);
        }
        else
        {
            //Nodes that get replicated should subscribe to NOKManager
            //So they know who the next master is upon master DC.
            NOKManager.Instance.Subscribe(this);
        }
    }

    //Meant to be overridden if a different naming convention is decided upon.
    //But really... stick to GUIDs yeah?
    private void SetName()
    {
        Name = System.Text.Encoding.ASCII.GetString(Guid.NewGuid().ToByteArray());
    }

    public void OnPeerConnected( int uid)
    {
        unconfirmed.Add(uid);
    }

    public void OnPeerDC( int uid)
    {
        unconfirmed.Remove(uid);
    }
    [Master]
    public void AckRPC()
    {
        unconfirmed.Remove(GetTree().GetRpcSenderId());
    }
}
