using Godot;
using System;


//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.

public class UserObserver : Node, IObserver<UserProvider>
{
    private UserProvider provider;

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    private CanvasItem mainMenu;
    private CanvasItem currentMenuNode = null;
    
    //This tracks whether we're spectating, using a character, driving.
    //Basically whatever camera this user is using. Operates at the provider level.
    private Node CurrentView = null;
    
    private SpawnManager PCManager;

    public override void _Ready()
    {
        PackedScene menuScene = GD.Load<PackedScene>("res://BasicScenes/GUI/MainMenu.tscn");
        mainMenu = (CanvasItem) GetNode("MainMenu");
        Input.SetMouseMode(Input.MouseMode.Visible);
        PCManager = (SpawnManager) GetNode("/root/GameRoot/PlayerCharacters");
    }

    public void Init(UserProvider provider)
    {
        this.provider = provider;
        //whenever a new provider is set, the team might change implicitly, even on startup.
        OnTeamChanged();
        provider.Connect(nameof(UserProvider.TeamChanged),this,nameof(OnTeamChanged));
        GetNode("MainMenu/TabContainer/Deployment/VBoxContainer/Spawn?/Option").Connect("pressed", this, nameof(SpawnPC));
    }

    public void OnTeamChanged()
    {
        //Whatever used to be the current view should be QueueFree'd by now.
        var spectatorScene = GD.Load<PackedScene>("res://BasicScenes/Player/Spectator/Spectator.tscn");
        CurrentView = spectatorScene.Instance();
        GetNode("/root/GameRoot/Map").AddChild(CurrentView);
    }

    public void SpawnPC()
    {
        CurrentView?.QueueFree();
        CurrentView = PCManager.Spawn("res://BasicScenes/Player/PlayerCharacter/PlayerCharacterProvider.tscn");
        
        provider.Rpc(nameof(UserProvider.SetCharacter),CurrentView.GetPath());
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
                    EmitSignal(nameof(SetInputEnabled), false);
                }
                else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    EmitSignal(nameof(SetInputEnabled), true);
                }
            }
        }
    }

}
