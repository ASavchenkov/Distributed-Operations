using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{    
    private SpawnManager players;

    public override void _Ready()
    {
        players = (SpawnManager) GetNode("Players");
        SpawnNewPlayer();

        players.Connect("Cleared",this, "SpawnNewPlayer");
    }

    public void SpawnNewPlayer()
    {
        players.Spawn("res://BasicScenes/Player/Player.tscn");
    }

    
}
