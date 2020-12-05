using Godot;
using System;

public class RandomSelector : Node
{
    
    public override void _Ready()
    {
        Networking.Instance.RTCMP.Connect("peer_connected", this, nameof(OnPeerConnected));
    }

    public void OnPeerConnected(int uid)
    {   
        NOKManager.Instance.ThisNOK = uid;
        Networking.Instance.RTCMP.Disconnect("peer_connected", this, nameof(OnPeerConnected));
        Networking.Instance.SignaledPeers[uid].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
        Networking.Instance.SignaledPeers[uid].Connect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));
    
    }

    public void OnPeerDC()
    {
        if(Networking.Instance.SignaledPeers.ContainsKey(NOKManager.Instance.ThisNOK))
        {
            Networking.Instance.SignaledPeers[NOKManager.Instance.ThisNOK].Disconnect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
            Networking.Instance.SignaledPeers[NOKManager.Instance.ThisNOK].Disconnect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));
        }
        
        int newNOK = -1;
        foreach(SignaledPeer p in Networking.Instance.SignaledPeers.Values)
        {
            if(p.CurrentState == SignaledPeer.ConnectionStateMachine.NOMINAL)
            {
                newNOK = p.UID;
                Networking.Instance.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnPeerDC));
                Networking.Instance.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.Delete),this, nameof(OnPeerDC));
                break;
            }
        }
        NOKManager.Instance.ThisNOK = newNOK;
        if(newNOK == -1)
            Networking.Instance.RTCMP.Connect("peer_connected", this, nameof(OnPeerConnected));
    }
}
