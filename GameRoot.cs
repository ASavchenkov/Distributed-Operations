using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{

    public bool inMainMenu = false;
    private Godot.Node currentMenuNode;
    //Because this is at the root of the game
    //This gets called after every other _Ready()
    //most importantly, after the MainMenu has called _Ready()
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        if(inputEvent is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("ui_cancel"))
            {
                inMainMenu = !inMainMenu;
                if(inMainMenu)
                {
                    Input.SetMouseMode(Input.MouseMode.Visible);
                }
                else
                {
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    
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
