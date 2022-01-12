using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using ReplicationAbstractions;

//Makes sure Next Of Kin (NOK) is synchronized between all peers,
//So that everyone knows who the master of each networked node is.
//Doesn't actually handle the selection of the NOK.
public class NOKManager : Node
{

    public static NOKManager Instance { get; private set;}

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
    
    public override void _Ready()
	{
        Instance = this;
        Networking.Instance.RTCMP.Connect("peer_connected",this,nameof(OnPeerConnected));
        Networking.Instance.RTCMP.Connect("peer_disconnected",this,nameof(OnPeerDC));
    }

    public void OnPeerConnected(int uid)
    {
        NOKs[uid] = new NOKSignaller();
        RpcId(uid, nameof(UpdateNOK), ThisNOK);
    }
    public void OnPeerDC(int uid)
    {
        NOKs[uid].trigger();
        NOKs.Remove(uid);
    }

    [Remote]
    public void UpdateNOK(int uid)
    {
        NOKs[GetTree().GetRpcSenderId()].uid = uid;
    }

    public void Subscribe (IReplicable n)
    {
        NOKs[n.GetNetworkMaster()].Connect(nameof(NOKSignaller.Transfer), n.rMember,nameof(ReplicationMember.OnNOKTransfer));
    }
    public void UnSubscribe (IReplicable n)
    {
        NOKs[n.GetNetworkMaster()].Disconnect(nameof(NOKSignaller.Transfer),n.rMember,nameof(ReplicationMember.OnNOKTransfer));
    }

}
