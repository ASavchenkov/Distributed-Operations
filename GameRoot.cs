using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;
    private Player player;
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        mainMenu = (CanvasItem) GetNode("MainMenu");
        player = (Player) GetNode("Player");
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
                    player.setInputEnabled(false);
                }else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    player.setInputEnabled(true);
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
