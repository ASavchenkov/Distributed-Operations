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
    
    [PuppetSync]
    public Node CurrentCharacter = null;

    [PuppetSync]
    public string Alias;

    private TDM tdm;
    private SpawnManager PCManager;

    public override void _Ready()
    {
        Alias = this.Name;  //Let player change it if they so wish.
                            //this.Name is a good default though.
        tdm = (TDM) GetNode("/root/GameRoot/TDM");
        PCManager = (SpawnManager) GetNode("/root/GameRoot/PlayerCharacters");

        if(!IsNetworkMaster())
            RpcId(GetNetworkMaster(), "RequestInit");
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
        tdm.EmitSignal("UpdateTDMLists");
    }

    [Master]
    public void RequestInit()
    {
        int uid = GetTree().GetRpcSenderId();
        RpcId(uid, "RemoteInit", ThisTeam, Score, VoteRestart, Alias);
    }

    //Only get's called on the master peer.
    public void SetTeam(Team team)
    {
        if(ThisTeam == team) return;

        ThisTeam = (Team) team;
        CurrentCharacter?.QueueFree();
        
        var specScene = GD.Load<PackedScene>("res://BasicScenes/Player/Spectator/Spectator.tscn");
        CurrentCharacter = specScene.Instance();
        GetNode("root/GameRoot/Map").AddChild(CurrentCharacter);
        Rpc("UpdateTeam", (int) ThisTeam, CurrentCharacter.GetPath());
        EmitSignal("TeamChanged");
    }
    [Puppet]
    public void UpdateTeam(int team, NodePath currentCharPath)
    {
        ThisTeam = (Team) team;
        CurrentCharacter = GetNode(currentCharPath);
        EmitSignal("TeamChanged");
    }
}
