using Godot;
using System;
using ReplicationAbstractions;

public class InventoryMenu : ViewportContainer
{
    public Spatial Workspace;
    //We need access to this to get "accessible" loot.
    public PlayerCharacterFPV pcFPV;

    public override void _Ready()
    {
        Workspace = (Spatial) GetNode("Viewport/Camera/InventoryWorkspace");
    }
}
