using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;
    private SpawnManager players;

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        mainMenu = (CanvasItem) GetNode("MainMenu");
        players = (SpawnManager) GetNode("Players");
        SpawnNewPlayer();

        players.Connect("Cleared",this, "SpawnNewPlayer");
    }

    public void SpawnNewPlayer()
    {
        players.Spawn("res://BasicScenes/Player/Player.tscn");
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        if(inputEvent is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("ui_cancel"))
            {
                //Then there is no menu, and we're going
                //to open the main menu.
                if(currentMenuNode is null)
                {
                    currentMenuNode = mainMenu;
                    currentMenuNode.Visible = true;
                    Input.SetMouseMode(Input.MouseMode.Visible);
                    EmitSignal("SetInputEnabled", false);
                }
                else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    EmitSignal("SetInputEnabled", true);
                }
            }
        }
    }
}
