using Godot;
using System;

//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.

public class UserObserver : Node
{
    public UserProvider provider {get; private set;}

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    private CanvasItem MainMenu;
    private CanvasItem currentMenuNode = null;
    
    //This tracks whether we're spectating, using a character, driving.
    //Basically whatever camera this user is using. Operates at the provider level.
    private Node CurrentView = null;
    
    public override void _Ready()
    {
        PackedScene menuScene = GD.Load<PackedScene>("res://BasicScenes/GUI/MainMenu.tscn");
        MainMenu = (CanvasItem) GetNode("MainMenu");
        Input.SetMouseMode(Input.MouseMode.Visible);
        
    }

    public void Subscribe(Node provider)
    {
        GD.Print("CALL TO USEROBSERVER INIT");
        this.provider = (UserProvider) provider;
        //whenever a new provider is set, the team might change implicitly, even on startup.
        OnTeamChanged();
        this.provider.Connect(nameof(UserProvider.TeamChanged),this,nameof(OnTeamChanged));
        MainMenu.GetNode("TabContainer/TDM/VBoxContainer/TeamChoice/Option").Connect("item_selected",provider, nameof(UserProvider.SetTeam));
        MainMenu.GetNode("TabContainer/Deployment/VBoxContainer/Spawn?/Option").Connect("pressed", this, nameof(SpawnPC));
    }

    public void OnTeamChanged()
    {
        //CurrentView may already have been freed, in which case do nothing.
        CurrentView?.QueueFree();
        var spectatorScene = GD.Load<PackedScene>("res://BasicScenes/Player/Spectator/Spectator.tscn");
        CurrentView = spectatorScene.Instance();
        GetNode("/root/GameRoot/Map").AddChild(CurrentView);
    }

    public void SpawnPC()
    {
        if(provider.ThisTeam == UserProvider.Team.Unassigned) return;

        CurrentView?.QueueFree();
        CurrentView = PlayerCharacterProvider.Factory.Instance();
        GetNode("/root/GameRoot/PlayerCharacters").AddChild(CurrentView);
        
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
                    currentMenuNode = MainMenu;
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
