using Godot;
using System;


//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.
public class UserObserver : Node, IObserver<UserProvider>
{
    private UserProvider provider;

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;
    
    public override void _Ready()
    {
        PackedScene menuScene = GD.Load<PackedScene>("res://BasicScenes/GUI/MainMenu.tscn");
        mainMenu = (CanvasItem) menuScene.Instance();
        AddChild(mainMenu);
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public void Init(UserProvider provider)
    {
        this.provider = provider;
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
