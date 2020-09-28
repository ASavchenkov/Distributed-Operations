using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{    
    
    private SpawnManager Users;
    private SpawnManager PlayerCharacters;
    private UserObserver LocalUser;

    public override void _Ready()
    {
        
        Users = (SpawnManager) GetNode("Users");
        UserProvider provider = (UserProvider) Users.Spawn("res://BasicScenes/Player/UserProvider.tscn");
        
        //UserObserver only has one observer.
        LocalUser = (UserObserver) provider.GenerateObserver(null);
        Users.Connect("Cleared",this, "AddUser");
    }

    public void AddUser()
    {
        UserProvider provider = (UserProvider) Users.Spawn("res://BasicScenes/Player/UserProvider.tscn");
        LocalUser.Init(provider);
    }

    
}
