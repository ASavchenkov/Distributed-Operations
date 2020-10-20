using Godot;
using System;
using System.Collections.Generic;


//Provider with the "authoritative" version of user data
//Stores some game specific information too for specators.

public class UserProvider : Node, IProvider
{
    
    [Export]
    public Dictionary<string,string> ObserverPaths;

    public enum Team {Unassigned, Red, Blue};
    public Team ThisTeam = Team.Unassigned;
    
    [Signal]
    public delegate void TeamChanged();
    //This is void because it doesn't really matter what team it is,
    //Just that current assets need to be removed from play
    //Redistributed to the team, or made "neutral"

    public int Score = 0;
    public bool VoteRestart = false;
    
    public Node CurrentCharacter = null;

    [PuppetSync]
    public string Alias;

    private SpawnManager PCManager;

    public override void _Ready()
    {
        Alias = this.Name;  //Let player change it if they so wish.
                            //this.Name is a good default though.
        
        PCManager = (SpawnManager) GetNode("/root/GameRoot/PlayerCharacters");

        var menu = GetNode("/root/GameRoot/UserObserver_1/MainMenu/TabContainer/TDM");
        Connect(nameof(TeamChanged), menu, nameof(TDMMenu.UpdateLists));
        
        if(!IsNetworkMaster())
            RpcId(GetNetworkMaster(), nameof(RequestInit));
        
    }

    public Node GenerateObserver(string name)
    {
        var observer = (IObserver<UserProvider>) GD.Load<PackedScene>(ObserverPaths[name]).Instance();
        observer.Init(this);
        return (Node) observer;
    }

    [Remote]
    public void RemoteInit(int team, int score, bool vote, string alias)
    {
        ThisTeam = (Team) team;
        Score = score;
        VoteRestart = vote;
        Alias = alias;
        EmitSignal(nameof(TeamChanged));
    }

    [Master]
    public void RequestInit()
    {
        int uid = GetTree().GetRpcSenderId();
        RpcId(uid, nameof(RemoteInit), ThisTeam, Score, VoteRestart, Alias);
    }


    [PuppetSync]
    public void UpdateTeam(int team)
    {
        ThisTeam = (Team) team;
        CurrentCharacter = null;
        EmitSignal(nameof(TeamChanged));
    }

    [PuppetSync]
    public void SetCharacter(NodePath path)
    {
        CurrentCharacter = GetNode(path);
    }
}
