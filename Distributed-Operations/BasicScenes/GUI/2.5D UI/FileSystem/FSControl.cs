using Godot;
using System;

public class FSControl : SpatialControl, IAnchored
{
    [Export]
    public AnchorMember anchorMember {get;set;}

    public override void _Ready()
    {
        anchorMember.Init(this);
        base._Ready();
    }
}
