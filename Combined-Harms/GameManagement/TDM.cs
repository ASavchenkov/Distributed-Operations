using Godot;
using System;


//Handles game resets and scoring.
//Operates on the peer level, so even when multiple players
//exist for a peer, there is only one TDM node.
//It's essentially a singleton.
public class TDM : Node
{
    private Node UserManager;
    
    int BlueScore = 0;
    int RedScore = 0;

    public override void _Ready()
    {
        UserManager = GetNode("/root/GameRoot/Users");
        

    }
}
