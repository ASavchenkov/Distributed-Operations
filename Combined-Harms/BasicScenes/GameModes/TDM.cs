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

    [Export]
    NodePath TeamSelPath;
    OptionButton TeamSelector;

    [Export]
    NodePath SpectatorPath;
    ItemList Spectators;

    [Export]
    NodePath RedPath;
    ItemList RedTeam;

    [Export]
    NodePath BluePath;
    ItemList BlueTeam;

    enum Team { Spectator, Red, Blue};

    int thisUID = 1;
    Dictionary<int, Team> teamTracker = new Dictionary<int, Team>();

    public override void _Ready()
    {
        TeamSelector = (OptionButton) GetNode(TeamSelPath);
        Spectators = (ItemList) GetNode(SpectatorPath);
        RedTeam = (ItemList) GetNode(RedPath);
        BlueTeam = (ItemList) GetNode(BluePath);
        GetTree().Connect("network_peer_connected",this, "OnPeerConnected");
    }

    private void UpdateLists()
    {
        Spectators.Clear();
        RedTeam.Clear();
        BlueTeam.Clear();
        foreach(KeyValuePair<int, Team> kvp in teamTracker )
        {
            switch(kvp.Value)
            {
                case Team.Spectator:
                    Spectators.AddItem(kvp.Key.ToString());
                    break;
                case Team.Red:
                    RedTeam.AddItem(kvp.Key.ToString());
                    break;
                case Team.Blue:
                    BlueTeam.AddItem(kvp.Key.ToString());
                    break;
            }
        }
    }

    //we just joined a new session, so we clear everything.
    public void OnConnectedToSession(int uid)
    {
        thisUID = GetTree().GetNetworkUniqueId();
        teamTracker.Clear();
        teamTracker.Add(uid, Team.Spectator);
        UpdateLists();
    }

    public void OnPeerConnected( int uid)
    {
        RpcId(uid, "UpdateTeam", (int) teamTracker[thisUID]);
    }

    // when WE select a team.
    public void OnTeamSelected(int team)
    {
        teamTracker[thisUID] = (Team) team;
        UpdateLists();
        Rpc("UpdateTeam",team);
    }

    //When someone else selects a team.
    [Remote]
    public void UpdateTeam(int team)
    {
        int sender = GetTree().GetRpcSenderId();
        teamTracker[sender] = (Team) team;
        UpdateLists();
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
     
    }
}
