using Godot;
using System;

using ReplicationAbstractions;

public class InventoryMenu : Spatial
{

    ILootPV root = null;
    Node rootObserver;

    public void Subscribe(ILootPV _root)
    {
        root = _root;
        rootObserver = EasyInstancer.GenObserver( (Node)root, root.ObserverPathLootPV);
        AddChild(rootObserver);
    }

}
