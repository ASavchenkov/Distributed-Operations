using Godot;
using System;

public class RandomSelector : Node
{
    NOKManager manager;
    Networking networking;
    public override void _Ready()
    {
        manager = (NOKManager) GetNode("/root/GameRoot/NOKManager");
        networking = (Networking) GetNode("/root/GameRoot/Networking");
        networking.RTCMP.Connect("peer_connected", this, nameof(OnPeerConnected));
    }

    public void OnPeerConnected(int uid)
    {   
        manager.ThisNOK = uid;
        networking.RTCMP.Disconnect("peer_connected", this, nameof(OnPeerConnected));
        networking.SignaledPeers[uid].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
        networking.SignaledPeers[uid].Connect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));
    
    }

    public void OnPeerDC()
    {
        networking.SignaledPeers[manager.ThisNOK].Disconnect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
        networking.SignaledPeers[manager.ThisNOK].Disconnect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));

        int newNOK = -1;
        foreach(SignaledPeer p in networking.SignaledPeers.Values)
        {
            if(p.CurrentState == SignaledPeer.ConnectionStateMachine.NOMINAL)
            {
                newNOK = p.UID;
                networking.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
                networking.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));
                break;
            }
        }
        manager.ThisNOK = newNOK;
        if(newNOK == -1)
            networking.RTCMP.Connect("peer_connected", this, nameof(OnPeerConnected));
    }
}
