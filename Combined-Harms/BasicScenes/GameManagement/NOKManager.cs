using Godot;
using System;
using System.Collections.Generic;

//Makes sure Next Of Kin (NOK) is synchronized between all peers,
//So that everyone knows who the master of each networked node is.
//Doesn't actually handle the selection of the NOK.
public class NOKManager : Node
{
    //expose as property so changes automatically trigger RPC.
    int _ThisNOK = -1;
    public int ThisNOK
    {
        get{return _ThisNOK;}
        set
        {
            _ThisNOK = value;
            Rpc(nameof(UpdateNOK), _ThisNOK);
        }
    }
    Dictionary<int, int> NOKs = new Dictionary<int, int>();
    Networking networking;

    public override void _Ready()
	{
        networking = (Networking) GetNode("/root/GameRoot/Networking");
        networking.RTCMP.Connect("peer_connected",this,nameof(OnPeerConnected));
        networking.RTCMP.Connect("peer_disconnected",this,nameof(OnPeerDC));
    }

    [Signal]
    public delegate void TransferToNOK(int peer, int NOK);

    public void OnPeerConnected(int uid)
    {
        RpcId(uid, nameof(RequestNOK));
    }
    public void OnPeerDC(int uid)
    {
        EmitSignal(nameof(TransferToNOK), uid, NOKs[uid]);
        NOKs.Remove(uid);
    }

    //Peers call this when they first connect to us.
    [Remote]
    public void RequestNOK()
    {
        int sender = GetTree().GetRpcSenderId();
        RpcId(sender, nameof(UpdateNOK), ThisNOK);
    }

    [Remote]
    public void UpdateNOK(int NOK)
    {
        NOKs[GetTree().GetRpcSenderId()] = NOK;
    }
}
