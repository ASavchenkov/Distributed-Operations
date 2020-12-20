using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{    
    
    private Node Users;
    private UserObserver LocalUser;

    public override void _Ready()
    {
        
        Users = GetNode("Users");
        
        //UserProvider only has one observer,
        //and it's a permanent node in the SceneTree
        LocalUser = (UserObserver) GetNode("UserObserver_1");
        AddUser();
        Users.Connect("Cleared",this, "AddUser");

        
    }
    public void AddUser()
    {
        UserProvider provider = UserProvider.Factory.Instance();
        LocalUser.Init(provider);
    }
}
