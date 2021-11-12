using Godot;
using System;

public class FSControl : SpatialControl, IAnchored
{
    public AnchorMember aMember {get;set;}

    public override void _Ready()
    {
        aMember = new AnchorMember(this);
        aMember.AnchorRight = 0.25f;
        base._Ready();
    }
}
