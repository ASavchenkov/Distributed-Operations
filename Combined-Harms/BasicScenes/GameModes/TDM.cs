using Godot;
using System;
using System.Collections.Generic;

/*
    Team management
    Spawning
    Scoring
*/
public class TDM : Node
{

    CenterContainer TDMTab;

    enum Team { Spectator, Red, Blue};

    int thisUID;
    Dictionary<int, Team> teamTracker = new Dictionary<int, Team>();

    public override void _Ready()
    {
        thisUID = GetTree().GetNetworkUniqueId();
        TDMTab = (CenterContainer) GetNode("/root/GameRoot/MainMenu/TabContainer/TDM");
        GetTree().Connect("network_peer_connected",this, "OnPeerConnected");
    }

    //we just joined a new session, so we clear everything.
    public void OnConnectedToSession(int uid)
    {
        teamTracker.Clear();
        teamTracker.Add(uid, Team.Spectator);
    }

    public void OnPeerConnected( int uid)
    {
        RpcId(uid, "UpdateTeam", (int) teamTracker[thisUID]);
    }

    [Remote]
    public void UpdateTeam(int team)
    {
        int sender = GetTree().GetRpcSenderId();
        teamTracker[sender] = (Team) team;
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
     
    }
}
