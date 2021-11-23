
using Godot;
using System;

public class FolderContents : SpatialVBoxContainer, IAnchored
{
    [Export]
    public AnchorMember anchorMember {get;set;}

    public override void _Ready()
    {
        anchorMember.Init(this);
        base._Ready();
    }
}
