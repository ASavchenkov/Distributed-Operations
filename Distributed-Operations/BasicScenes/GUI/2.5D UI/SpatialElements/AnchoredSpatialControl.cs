using Godot;
using System;

public class AnchoredSpatialControl : SpatialControl, IAnchored
{
    [Export]
    public AnchorMember anchorMember {get;set;}
    
    public override void _Ready()
    {
        anchorMember.Init(this);
    }
}
