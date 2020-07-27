using Godot;
using System;


//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.
public class Player : Node
{
    public enum Team { Spectator, Red, Blue};

    public Team team = Team.Spectator;
    public int score = 0;
    public bool voteRestart = false;
    public bool alive = false;
    public string Alias;

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;

    public override void _Ready()
    {
        Alias = this.Name;

        Input.SetMouseMode(Input.MouseMode.Visible);
        mainMenu = (CanvasItem) GetNode("MainMenu");
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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
