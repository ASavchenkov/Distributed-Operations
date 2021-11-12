using Godot;
using System;

public class FSControl : SpatialControl, IAnchored
{
    [Export]
    public AnchorMember aMember {get;set;}

    public override void _Ready()
    {
        aMember.Init(this);
        base._Ready();
    }
}
