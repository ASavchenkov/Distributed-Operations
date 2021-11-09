using Godot;
using System;

//For when you just need an intermediate SpatialControl
//to act as a container.
//Not likely you will ever use it though.
public class AnchoredSpatial : SpatialControl,  IAnchored
{
    [Export]
    public AnchorMember aMember {get;set;}

    public override void _Ready()
    {
        aMember.Init(this);
    }
}
