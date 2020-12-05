using Godot;
using System;

public class NetworkingTab : CenterContainer
{
    private ItemList peerList;
    
    public override void _Ready()
    {
        peerList = (ItemList) GetNode("VBoxContainer/ScrollContainer/PeerList");
        
        //connect all the signals to Networking.
        LineEdit HostURL = (LineEdit) GetNode("VBoxContainer/HostURL/LineEdit");
        HostURL.Connect("text_changed", Networking.Instance, "_SetURL");

        LineEdit Secret = (LineEdit) GetNode("VBoxContainer/Secret/LineEdit");
        Secret.Connect("text_changed", Networking.Instance, "_SetSecret");

        Button JoinSession = (Button) GetNode("VBoxContainer/Join");
        JoinSession.Connect("pressed", Networking.Instance, "_JoinMesh");
        
        Button StartServer = (Button) GetNode("VBoxContainer/ServerEnabled/Start");
        StartServer.Connect("pressed", Networking.Instance, "_StartServer");

        Button StopServer = (Button) GetNode("VBoxContainer/ServerEnabled/Stop");
        StopServer.Connect("pressed", Networking.Instance, "_StopServer");
        
    }

    public void _DisplayPeers()
    {
        GD.Print("DISPLAYING PEERS");
        peerList.Clear();
        
        foreach(int uid in Networking.Instance.RTCMP.GetPeers().Keys)
		{
			string peerString = uid.ToString() + ": " + ((bool) Networking.Instance.RTCMP.GetPeer(uid)["connected"]).ToString() + "\n";
            peerList.AddItem(peerString);
        }

    }
    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
