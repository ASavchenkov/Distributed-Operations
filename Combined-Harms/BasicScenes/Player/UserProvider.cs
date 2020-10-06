using Godot;
using System;
using System.Collections.Generic;

public class UserProvider : Node, IProvider
{

    public event NotifyProviderEnd ProviderEnd;

    [Export]
    public Dictionary<string,string> ObserverPaths;

    public enum Team { Spectator, Red, Blue};
    public Team ThisTeam = Team.Spectator;
    
    public int Score = 0;
    public bool VoteRestart = false;
    public bool Alive = false;
    public string Alias;

    private TDM tdm;

    public override void _Ready()
    {
        Alias = this.Name;  //Let player change it if they so wish.
                            //this.Name is a good default though.
        tdm = (TDM) GetNode("/root/GameRoot/TDM");
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
    public void RemoteInit(int team, int score, bool vote, bool alive, string alias)
    {
        ThisTeam = (Team) team;
        Score = score;
        VoteRestart = vote;
        Alive = alive;
        Alias = alias;
        tdm.EmitSignal("UpdateTDMLists");
    }

    [Master]
    public void RequestInit()
    {
        int uid = GetTree().GetRpcSenderId();
        RpcId(uid, "RemoteInit", ThisTeam, Score, VoteRestart, Alive, Alias);
    }

    [RemoteSync]
    public void UpdateTeam(int team)
    {
        ThisTeam = (Team) team;
        tdm.EmitSignal("UpdateTDMLists");
    }

    [RemoteSync]
    public void UpdateVote(bool vote)
    {
        bool oldVote = VoteRestart;
        VoteRestart = vote;
        if(oldVote!=VoteRestart)
            tdm.CheckQuorum();
    }

    protected override void Dispose(bool disposing)
    {
        ProviderEnd?.Invoke();
        base.Dispose(disposing);
    }
}
