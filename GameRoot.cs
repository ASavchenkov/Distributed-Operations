using Godot;
using System;

public class GameRoot : Spatial
{

    // Called when the node enters the scene tree for the first time.
    public bool inMainMenu = false;

    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent inputEvent)
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
