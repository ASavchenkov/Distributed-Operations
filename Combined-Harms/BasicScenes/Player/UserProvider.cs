using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

//Provider with the "authoritative" version of user data
//Stores some game specific information too for specators.

public class UserProvider : Node, IReplicable, IFPV
{

    public ReplicationMember rMember {get; set;}

    public static NodeFactory<UserProvider> Factory = 
        new NodeFactory<UserProvider>("res://BasicScenes/Player/UserProvider.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}

    [Export]
    public string ObserverPathFPV {get;set;}

    public enum Team {Unassigned, Red, Blue};
    public Team ThisTeam = Team.Red;
    
    [Signal]
    public delegate void TeamChanged();
    //This is void because it doesn't really matter what team it is,
    //Just that current assets need to be removed from play
    //Redistributed to the team, or made "neutral"

    public int Score = 0;
    public bool VoteRestart = false;
    
    public Node CurrentCharacter = null;

    public string Alias;
    
    public override void _Ready()
    {
        Connect("tree_exiting", this, nameof(OnQueueFree));

        GD.Print("Master UID: ", GetNetworkMaster());
        this.ReplicableReady();
        
        Alias = this.Name;  //Let player change it if they so wish.
                            //this.Name is a good default though.
        GD.Print("Alias: ", Alias);
        var menu = GetNode("/root/UserObserver_1/MainMenu/TabContainer/TDM");
        
        Connect(nameof(TeamChanged), menu, nameof(TDMMenu.UpdateLists));
        
        if(!IsNetworkMaster())
        {
            RpcId(GetNetworkMaster(), nameof(RequestInit));
        }
        
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

    //Call locally
    public void SetTeam(int team)
    {
        if(ThisTeam == (UserProvider.Team) team) return;

        if(IsInstanceValid(CurrentCharacter))
            CurrentCharacter.QueueFree();
            
        CurrentCharacter = null;
        Rpc(nameof(UpdateTeam), team);
    }

    [PuppetSync]
    public void UpdateTeam(int team)
    {
        GD.Print(Name, ": Changed team");
        ThisTeam = (Team) team;
        CurrentCharacter = null;
        EmitSignal(nameof(TeamChanged));
    }

    [PuppetSync]
    public void SetCharacter(NodePath path)
    {
        CurrentCharacter = GetNode(path);
    }

    public void OnNOKTransfer(int uid)
    {
        //This particular object can just be deleted (safely)
        //if the peer DCs.
        rMember.MasterDespawn();
    }
    public void OnQueueFree()
    {
        GD.PrintErr("Oh no we don't want to go!");
    }
}
