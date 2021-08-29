using Godot;
using System;
using System.Threading.Tasks;
using MessagePack;

using ReplicationAbstractions;

public class GameManager : Node
{
    private Node Users;
    private UserObserver LocalUser;

    public override void _Ready()
    {

        LocalUser = (UserObserver) GetNode("/root/UserObserver_1");
        //We've essentially connected to our own session when we start the application
        GetNode("/root").Connect("ready", this, nameof(OnConnectedToSession),
            new Godot.Collections.Array{-1});
        Networking.Instance.Connect(nameof(Networking.ConnectedToSession), this, nameof(OnConnectedToSession));

    }

    public void OnConnectedToSession(int uid)
    {
        
        GD.Print("This should only get called once");
        //If this is the start of the application, disconnect the signal from root.
        if(uid == -1)
            GetNode("/root").Disconnect("ready",this, nameof(OnConnectedToSession));
        else
            EasyInstancer.NetworkID = uid;   
        
        //Manually replace because the default seems to do it async
        //and we need to do this synchronously.
        GetNode("/root/GameRoot").Free();
        var gameRoot = EasyInstancer.Instance<Node>("res://GameRoot.tscn");
        GetNode("/root").AddChild(gameRoot);

        Users = gameRoot.GetNode("Users");
        UserProvider provider = UserProvider.Factory.Instance();
        Users.AddChild(provider);
        LocalUser.Subscribe(provider);
    }




}
