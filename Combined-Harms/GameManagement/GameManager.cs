using Godot;
using System;

public class GameManager : Node
{
    private Node Users;
    private UserObserver LocalUser;

    public override void _Ready()
    {
        Users = GetNode("/root/GameRoot/Users");
        
        //UserProvider only has one observer,
        //and it's a permanent node in the SceneTree
        LocalUser = (UserObserver) GetNode("/root/UserObserver_1");
        OnConnectedToSession(1);
        //We've essentially connected to our own session when we start the application
        Networking.Instance.Connect(nameof(Networking.ConnectedToSession), this, nameof(OnConnectedToSession));
    }

    public void OnConnectedToSession(int uid)
    {
        LocalUser?.provider?.rMember?.MasterDespawn();
        UserProvider provider = UserProvider.Factory.Instance();
        provider.SetNetworkMaster(GetTree().GetNetworkUniqueId());
        Users.AddChild(provider);
        LocalUser.Subscribe(provider);
    }
}
