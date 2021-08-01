using Godot;
using System;
using ReplicationAbstractions;

public class InventoryMenu : Spatial
{
    Spatial workspace;
    //We need access to this to get "accessible" loot.
    public PlayerCharacterFPV pcFPV;

    public override void _Ready()
    {
        workspace = (Spatial) GetNode("InventoryWorkspace");
        var pcObserver = EasyInstancer.GenObserver(pcFPV.provider, pcFPV.provider.ObserverPathLootPV);
        workspace.AddChild(pcObserver);
    }

}
