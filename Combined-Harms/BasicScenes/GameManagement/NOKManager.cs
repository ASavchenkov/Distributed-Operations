using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

//Makes sure Next Of Kin (NOK) is synchronized between all peers,
//So that everyone knows who the master of each networked node is.
//Doesn't actually handle the selection of the NOK.
public class NOKManager : Node
{

    public class NOKSignaller : Godot.Object
    {
        //who the next of kin is.
        //public because who else is going to change this?
        public int uid = -1;
        [Signal]
        public delegate void Transfer(int uid);
        public void trigger()
        {
            EmitSignal(nameof(Transfer), uid);
        }

        
    }

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
    Dictionary<int, NOKSignaller> NOKs = new Dictionary<int, NOKSignaller>();
    Networking networking;

    public override void _Ready()
	{
        networking = (Networking) GetNode("/root/GameRoot/Networking");
        networking.RTCMP.Connect("peer_connected",this,nameof(OnPeerConnected));
        networking.RTCMP.Connect("peer_disconnected",this,nameof(OnPeerDC));
    }

    public void OnPeerConnected(int uid)
    {
        NOKs[uid] = new NOKSignaller();
        RpcId(uid, nameof(RequestNOK));
    }
    public void OnPeerDC(int uid)
    {
        NOKs[uid].trigger();
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
    public void UpdateNOK(int uid)
    {
        NOKs[GetTree().GetRpcSenderId()].uid = uid;
    }

    public void Subscribe (Node n)
    {
        Debug.Assert(n is INOKTransferrable);
        NOKs[n.GetNetworkMaster()].Connect(nameof(NOKSignaller.Transfer),n,nameof(INOKTransferrable.OnNOKTransfer));
    }
    public void UnSubscribe (Node n)
    {
        Debug.Assert(n is INOKTransferrable);
        NOKs[n.GetNetworkMaster()].Disconnect(nameof(NOKSignaller.Transfer),n,nameof(INOKTransferrable.OnNOKTransfer));
    }

    //Poor mans default interface implementation.
    //We're not on c# 8 yet so the default function is here
    //and needs to explicitly be invoked still.
    public void ChangeMaster(Node n, int uid)
    {
        UnSubscribe(n);
        n.SetNetworkMaster(uid);
        Subscribe(n);
    }
}
