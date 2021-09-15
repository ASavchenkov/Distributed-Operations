using Godot;
using System;

using ReplicationAbstractions;
//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.

public class UserObserver : Node, ITakesInput, IObserver
{
    public UserProvider provider {get; private set;}

    public InputClaims Claims {get;set;} = new InputClaims();

    private CanvasItem MainMenu;
    private ViewportContainer inventoryMenu;
    private CanvasItem currentMenuNode = null;
    
    //This tracks whether we're spectating, using a character, driving.
    //Basically whatever camera this user is using. Operates at the provider level.
    private Node CurrentView = null;
    
    public override void _Ready()
    {
        MainMenu = (CanvasItem) GetNode("MainMenu/MainMenu");
        Input.SetMouseMode(Input.MouseMode.Visible);
        MainMenu.GetNode("TabContainer/Deployment/VBoxContainer/Spawn?/Option").Connect("pressed", this, nameof(SpawnPC));
        
        inventoryMenu = EasyInstancer.Instance<ViewportContainer>("res://BasicScenes/GUI/2.5D UI/InventoryMenu.tscn");
        
        //We are the last ones in line, since we only care about Esc key.
        //Any other system that uses it gets it first.
        Claims.Claims.Add("ui_cancel");
        InputPriorityServer.Base.Subscribe(this, BaseRouter.gameManagement);
    }

    public void Subscribe(object _provider)
    {
        GD.Print("CALL TO USEROBSERVER INIT");
        provider = (UserProvider) _provider;
        //whenever a new provider is set, the team might change implicitly, even on startup.
        OnTeamChanged();
        provider.Connect(nameof(UserProvider.TeamChanged),this,nameof(OnTeamChanged));
        MainMenu.GetNode("TabContainer/TDM/VBoxContainer/TeamChoice/Option").Connect("item_selected", provider, nameof(UserProvider.SetTeam));
    }

    public void OnTeamChanged()
    {
        //CurrentView may already have been freed, in which case do nothing.
        if(IsInstanceValid(CurrentView))
            CurrentView.QueueFree();
        var spectatorScene = GD.Load<PackedScene>("res://BasicScenes/Player/Spectator/Spectator.tscn");
        CurrentView = spectatorScene.Instance();
        GetNode("/root/GameRoot/Map").AddChild(CurrentView);
    }

    public void SpawnPC()
    {
        if(provider.ThisTeam == UserProvider.Team.Unassigned) return;

        if(IsInstanceValid(CurrentView))
        {
            CurrentView.QueueFree();
            //Call to make sure that it unsubs from input stuff.
            //(since there's no "OnQueueFree" signal or function.)
            CurrentView.Dispose();
        }
        
        CurrentView = PlayerCharacterProvider.Factory.Instance();
        GetNode("/root/GameRoot/PlayerCharacters").AddChild(CurrentView);
        provider.Rpc(nameof(UserProvider.SetCharacter),CurrentView.GetPath());
    }

    public bool OnInput(InputEvent inputEvent)
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
                }
                else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                }
                return true;
            }
            else if(keyEvent.IsActionPressed("Inventory"))
            {
                if( inventoryMenu.IsInsideTree())
                    GetNode("/root/GameRoot").RemoveChild(inventoryMenu);
                else
                    GetNode("/root/GameRoot").AddChild(inventoryMenu);
                return true;
            }
        }
        return false;
    }

}
