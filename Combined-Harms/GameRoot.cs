using Godot;
using System;
using MessagePack;

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
        Networking.Instance.Connect(nameof(Networking.ConnectedToSession), this, nameof(OnConnectedToSession));
    }

    public void OnConnectedToSession(int uid)
    {

    }
    public void AddUser()
    {
        UserProvider provider = UserProvider.Factory.Instance();
        Users.AddChild(provider);
        LocalUser.Subscribe(provider);
    }
}
