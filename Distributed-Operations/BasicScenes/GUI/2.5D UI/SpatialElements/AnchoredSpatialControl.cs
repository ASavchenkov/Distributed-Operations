using Godot;
using System;

public class AnchoredSpatialControl : SpatialControl, IAnchored
{
    [Export]
    public AnchorMember anchorMember {get;set;}
}
