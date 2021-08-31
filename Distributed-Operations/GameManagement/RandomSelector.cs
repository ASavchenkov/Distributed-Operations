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
        Networking.Instance.SignaledPeers[uid].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnNOKDC));
        Networking.Instance.SignaledPeers[uid].Connect(nameof(SignaledPeer.Delete),this, nameof(OnNOKDC));
    
    }

    public void OnNOKDC()
    {
        GD.Print("CALL TO ONNOKDC");
        if(Networking.Instance.SignaledPeers.ContainsKey(NOKManager.Instance.ThisNOK))
        {
            Networking.Instance.SignaledPeers[NOKManager.Instance.ThisNOK].Disconnect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnNOKDC));
            Networking.Instance.SignaledPeers[NOKManager.Instance.ThisNOK].Disconnect(nameof(SignaledPeer.Delete),this, nameof(OnNOKDC));
        }
        
        int newNOK = -1;
        foreach(SignaledPeer p in Networking.Instance.SignaledPeers.Values)
        {
            if(p.CurrentState == SignaledPeer.ConnectionStateMachine.NOMINAL)
            {
                newNOK = p.UID;
                Networking.Instance.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.ConnectionLost),this, nameof(OnNOKDC));
                Networking.Instance.SignaledPeers[newNOK].Connect(nameof(SignaledPeer.Delete),this, nameof(OnNOKDC));
                break;
            }
        }
        NOKManager.Instance.ThisNOK = newNOK;
        if(newNOK == -1)
            Networking.Instance.RTCMP.Connect("peer_connected", this, nameof(OnPeerConnected));
    }
}
