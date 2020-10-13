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
    NodePath VoteCounterPath;
    Label VoteCounter;

    [Export]
    NodePath VotePath;

    UserObserver ThisPlayer;
    private Node PlayerSpawnManager;

    public override void _Ready()
    {
        Spectators = (ItemList) GetNode(SpectatorPath);
        RedTeam = (ItemList) GetNode(RedPath);
        BlueTeam = (ItemList) GetNode(BluePath);
        VoteCounter = (Label) GetNode(VoteCounterPath);

        ThisPlayer = (UserObserver) GetNode("../../..");
        PlayerSpawnManager = GetNode("/root/GameRoot/Players");

        GetNode("/root/GameRoot/TDM").Connect("UpdateTDMLists",this,"UpdateLists");
        GetNode("/root/GameRoot/TDM").Connect("UpdateVotes",this,"UpdateVotes");
        GetNode(VotePath).Connect("toggled", this, "VoteToggled");

        UpdateLists();
    }

    public void UpdateLists()
    {
        Spectators.Clear();
        RedTeam.Clear();
        BlueTeam.Clear();

        Godot.Collections.Array players = PlayerSpawnManager.GetChildren();

        foreach( Node p in players)
        {
            UserProvider player = (UserProvider) p;
            switch(player.ThisTeam)
            {
                case UserProvider.Team.Unassigned:
                    Spectators.AddItem(player.Name);
                    break;
                case UserProvider.Team.Red:
                    RedTeam.AddItem(player.Name);
                    break;
                case UserProvider.Team.Blue:
                    BlueTeam.AddItem(player.Name);
                    break;
            }
        }
    }

    public void VoteToggled(bool vote)
    {
        ThisPlayer.Rpc("UpdateVote", vote);
    }

    public void UpdateVotes(int votes, float needed, int total)
    {
        VoteCounter.Text = $"{votes}/{total}, {needed} votes needed.";
    }

    // when WE select a team.
    public void OnTeamSelected(int team)
    {
        ThisPlayer.Rpc("UpdateTeam",team);
    }

    public void OnToggleVoteRestart( bool vote)
    {
        ThisPlayer.Rpc("UpdateVote", vote);
    }
}
