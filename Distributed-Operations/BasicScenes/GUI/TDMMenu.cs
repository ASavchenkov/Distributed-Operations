using Godot;
using System;
using System.Collections.Generic;

public class TDMMenu : Node
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
    private Node UserManager;

    public override void _Ready()
    {   
        Spectators = (ItemList) GetNode(SpectatorPath);
        RedTeam = (ItemList) GetNode(RedPath);
        BlueTeam = (ItemList) GetNode(BluePath);
        VoteCounter = (Label) GetNode(VoteCounterPath);

        UpdateLists();
    }

    public void UpdateLists()
    {
        Spectators.Clear();
        RedTeam.Clear();
        BlueTeam.Clear();


        UserManager = GetNode("/root/GameRoot/GameWorld/Users");
        Godot.Collections.Array users = UserManager.GetChildren();

        foreach( Node p in users)
        {
            UserProvider user = (UserProvider) p;
            switch(user.ThisTeam)
            {
                case UserProvider.Team.Unassigned:
                    Spectators.AddItem(user.Alias);
                    break;
                case UserProvider.Team.Red:
                    RedTeam.AddItem(user.Alias);
                    break;
                case UserProvider.Team.Blue:
                    BlueTeam.AddItem(user.Alias);
                    break;
            }
        }
    }

    public void UpdateVotes(int votes, float needed, int total)
    {
        VoteCounter.Text = $"{votes}/{total}, {needed} votes needed.";
    }


}
