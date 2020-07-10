using Godot;
using System;

public class NetworkingTab : CenterContainer
{
    private ItemList peerList;
    private Networking networking;
    public override void _Ready()
    {
        peerList = (ItemList) GetNode("VBoxContainer/ScrollContainer/PeerList");
        networking = (Networking) GetNode("/root/GameRoot/Networking");
    }

    public void _DisplayPeers()
    {
        GD.Print("DISPLAYING PEERS");
        peerList.Clear();
        
        foreach(int uid in networking.RTCMP.GetPeers().Keys)
		{
			string peerString = uid.ToString() + ": " + ((bool) networking.RTCMP.GetPeer(uid)["connected"]).ToString() + "\n";
            peerList.AddItem(peerString);
        }

    }
    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
