using Godot;
using System;


//Handles high level management of the game.
//Specifically, how the escape key interacts with menus
public class GameRoot : Spatial
{

    private Godot.CanvasItem mainMenu;
    private Godot.CanvasItem currentMenuNode = null;
    private LocalPlayer player;

    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        mainMenu = (CanvasItem) GetNode("MainMenu");
        player = (LocalPlayer) GetNode("Players/1");

        //for deciding who the new master of objects might be.
        GetTree().Connect("network_peer_connected", this, "PeerConnected");
        GetTree().Connect("network_peer_disconnected", this, "PeerDisconnected");
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        
        if(inputEvent is InputEventKey keyEvent)
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
                    player.setInputEnabled(false);
                }
                else{
                    currentMenuNode.Visible = false;
                    currentMenuNode = null;
                    Input.SetMouseMode(Input.MouseMode.Captured);
                    player.setInputEnabled(true);
                }
            }
        }
    }

    public void PeerConnected(int uid)
    {
        //Tell the other peer that we want them to create a representation
        //of our player.
        GD.Print("Calling AddRemotePlayer");
        Rpc("AddRemotePlayer",GetTree().NetworkPeer.GetUniqueId());
    }

    [Remote]
    public void AddRemotePlayer(int uid)
    {
        var playerScene = GD.Load<PackedScene>("res://BasicScenes/Player/RemotePlayer.tscn");
        var playerNode = playerScene.Instance();
        playerNode.Name = uid.ToString();
        playerNode.SetNetworkMaster(uid);
        GetNode("Players").AddChild(playerNode);
    }

    //Make sure our peer has the right name
    public void _OnUIDChanged(int uid)
    {
        player.Name = uid.ToString();
        player.SetNetworkMaster(uid);
    }

    //Every N seconds, each GameRoot will ping the rest to show they're still connected.
    [Remote]
    public void Ping()
    {

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
