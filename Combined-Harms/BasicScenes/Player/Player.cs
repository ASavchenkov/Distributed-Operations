using Godot;
using System;


//Overall root node for the player,
//whether they are in the menu, spectating, or currently playing.
public class Player : Node
{
    public enum Team { Spectator, Red, Blue};

    public Team ThisTeam = Team.Spectator;
    public int Score = 0;
    public bool VoteRestart = false;
    public bool Alive = false;
    public string Alias;

    [Signal]
    public delegate void SetInputEnabled(bool enabled);

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;
    private TDM tdm;
    public override void _Ready()
    {
        
        if(IsNetworkMaster())
        {
            PackedScene menuScene = GD.Load<PackedScene>("res://BasicScenes/GUI/MainMenu.tscn");
            mainMenu = (CanvasItem) menuScene.Instance();
            mainMenu.Name = "MainMenu";
            AddChild(mainMenu);
        }

        Alias = this.Name;
        Input.SetMouseMode(Input.MouseMode.Visible);
        tdm = (TDM) GetNode("/root/GameRoot/TDM");
        if(!IsNetworkMaster())
            Rpc("RequestInit");
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

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        if(IsNetworkMaster() && inputEvent is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("ui_cancel"))
            {
                //Then there is no menu, and we're going
                //to open the main menu.
                if(currentMenuNode is null)
                {
                    currentMenuNode = mainMenu;
                    currentMenuNode.Visible = true;
                    Input.SetMouseMode(Input.MouseMode.Visible);
                    EmitSignal("SetInputEnabled", false);
                }
                else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    EmitSignal("SetInputEnabled", true);
                }
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
