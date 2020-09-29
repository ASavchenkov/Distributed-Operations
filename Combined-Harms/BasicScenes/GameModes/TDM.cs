using Godot;
using System;


//Handles game resets, scoring, spectating, and spawning.
//Operates on the peer level, so even when multiple players
//exist for a peer, there is only one TDM node.
//It's essentially a global object.
public class TDM : Node
{
    private Node UserManager;
    
    [Signal]
    public delegate void UpdateTDMLists();

    [Signal]
    public delegate void UpdateVotes(int votes, int needed, int total);

    [Export]
    float Quorum = 2/3;

    int BlueScore = 0;
    int RedScore = 0;

    public override void _Ready()
    {
        UserManager = GetNode("/root/GameRoot/Users");

    }

    public void CheckQuorum()
    {
        int totalVotes = 0;
        int totalPlayers = UserManager.GetChildCount();
        Godot.Collections.Array players = UserManager.GetChildren();
        foreach( Node p in players)
        {
            UserProvider player = (UserProvider) p;
            if(player.VoteRestart)
                totalVotes++;
        }
        if( (float)totalVotes > (float) totalPlayers * Quorum)
        {
            RedScore = 0;
            BlueScore = 0;
        }
        EmitSignal("UpdateVotes", totalVotes,(Quorum * (float) totalPlayers), totalPlayers);
        
    }

}
