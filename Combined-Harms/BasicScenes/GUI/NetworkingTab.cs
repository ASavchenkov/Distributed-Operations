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

        //connect all the signals to Networking.
        LineEdit HostURL = (LineEdit) GetNode("VBoxContainer/HostURL/LineEdit");
        HostURL.Connect("text_changed", networking, "_SetURL");

        LineEdit Secret = (LineEdit) GetNode("VBoxContainer/Secret/LineEdit");
        Secret.Connect("text_changed", networking, "_SetSecret");

        Button JoinSession = (Button) GetNode("VBoxContainer/Join");
        JoinSession.Connect("pressed", networking, "_JoinMesh");
        
        Button StartServer = (Button) GetNode("VBoxContainer/ServerEnabled/Start");
        StartServer.Connect("pressed", networking, "_StartServer");

        Button StopServer = (Button) GetNode("VBoxContainer/ServerEnabled/Stop");
        StopServer.Connect("pressed", networking, "_StopServer");
        
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
