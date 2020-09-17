using Godot;
using System;

public abstract class RifleProvider : Node
{
    Node ParentProvider = null;
    Node MagazineProvider = null;
    Node TopRailAttachProvider = null;
    //Needs to be an IFPVProvider but I can't figure out how to constrain object.
    PackedScene FPVBase;

    public override void _Ready()
    {
        
    }

    public Node GenerateFPVObserver()
    {
        Node instance = FPVBase.Instance();
        if(!(TopRailAttachProvider is null))
        {
            Node topRail = instance.GetNode("Origin/Gun/TopRail");
            Node topAttach = ((IFPVProvider) TopRailAttachProvider).GenerateFPVObserver();
            topRail.AddChild(topAttach);
        }
        return instance;
    }

    [RemoteSync]
    public void SetMaster(int uid)
    {
        SetNetworkMaster(uid);
    }

    //Please only call this if you're the master.
    //Honor system.
    [PuppetSync]
    public void SetParent(NodePath path)
    {
        ParentProvider = GetNode(path);
    }
    [PuppetSync]
    public void SetMagazine(NodePath path)
    {
        MagazineProvider = GetNode(path);
    }
    [PuppetSync]
    public void SetTopRail(NodePath path)
    {
        TopRailAttachProvider = GetNode(path);
    }
}
