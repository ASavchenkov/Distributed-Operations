using Godot;
using System;
using System.Collections.Generic;

public class TeamManager : Node
{


    [Export]
    NodePath SpectatorPath;
    ItemList Spectators;

    [Export]
    NodePath RedPath;
    ItemList RedTeam;

    [Export]
    NodePath BluePath;
    ItemList BlueTeam;

    [Export]
    NodePath VoteRestartLabelPath;
    Label VoteRestartLabel;

    public enum Team { Spectator, Red, Blue};
    int BlueScore = 0;
    int RedScore = 0;

    int thisUID = 1;
    
    private class PlayerStat
    {
        public Team team;
        public int score;
        public bool voteRestart;
        

        public PlayerStat(Team team)
        {
            this.team = team;
            score = 0;
            voteRestart = false;
        }
    }
    int totalVotes = 0;
    [Export] float Quorum = 2/3;

    public override void _Ready()
    {
        Spectators = (ItemList) GetNode(SpectatorPath);
        RedTeam = (ItemList) GetNode(RedPath);
        BlueTeam = (ItemList) GetNode(BluePath);
        GetTree().Connect("network_peer_connected",this, "OnPeerConnected");

        UpdateLists();
    }

    private void UpdateLists()
    {
        Spectators.Clear();
        RedTeam.Clear();
        BlueTeam.Clear();
        foreach(KeyValuePair<int, PlayerStat> kvp in playerStats )
        {
            switch(kvp.Value.team)
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
        thisUID = uid;
        playerStats.Clear();
        playerStats.Add(uid, new PlayerStat(Team.Spectator));
        UpdateLists();
    }

    public void OnPeerConnected( int uid)
    {
        playerStats.Add(uid, new PlayerStat(Team.Spectator));
        RpcId(uid, "UpdateTeam", (int) playerStats[thisUID].team);
    }

    // when WE select a team.
    public void OnTeamSelected(int team)
    {
        playerStats[thisUID].team = (Team) team;
        UpdateLists();
        Rpc("UpdateTeam",team);
    }

    //When someone else selects a team.
    [Remote]
    public void UpdateTeam(int team)
    {
        int sender = GetTree().GetRpcSenderId();
        playerStats[sender].team = (Team) team;
        UpdateLists();
    }

    public void OnToggleVoteRestart( bool vote)
    {
        Rpc("UpdateRestartVote", vote);
    }

    [RemoteSync]
    public void UpdateRestartVote(bool vote)
    {
        int sender = GetTree().GetRpcSenderId();
        if(playerStats[sender].voteRestart != vote)
        {
            if(vote) totalVotes++;
            else totalVotes--;
        }

        playerStats[sender].voteRestart = vote;
        if( (float)totalVotes * Quorum > (float)playerStats.Count)
        {
            RedScore = 0;
            BlueScore = 0;
        }

        
    }
}
