using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{    
    
    private SpawnManager Users;
    private UserObserver LocalUser;

    public override void _Ready()
    {
        
        Users = (SpawnManager) GetNode("Users");
        UserProvider provider = (UserProvider) Users.Spawn("res://BasicScenes/Player/UserProvider.tscn");
        
        //UserProvider only has one observer,
        //and it's a permanent node in the SceneTree
        LocalUser = (UserObserver) GetNode("UserObserver_1");
        LocalUser.Init(provider);
        Users.Connect("Cleared",this, "AddUser");

        
    }
    public void AddUser()
    {
        UserProvider provider = (UserProvider) Users.Spawn("res://BasicScenes/Player/UserProvider.tscn");
        LocalUser.Init(provider);
    }
}
