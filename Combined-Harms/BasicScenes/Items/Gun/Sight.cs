using Godot;
using System;

public abstract class Sight : Spatial
{
    //These may be the same node, or different ones
    //for ironsights "RotationNode" will probably be the front post
    // and "SightNode" will be the rear sight.

    //For single plane optics, both will be the plane.
    Spatial RotationNode;
    Spatial SightNode;

    [Export]
    NodePath EyeReliefPath;
    RemoteTransform EyeRelief;
    public Position3D RemoteEyeRelief;


    public override void _Ready()
    {
        EyeRelief = (RemoteTransform) GetNode(EyeReliefPath);
        RemoteEyeRelief = (Position3D) EyeRelief.GetNode(EyeRelief.RemotePath);
    }


    //Zero the sight:
    //  Set the rotation
    //  compute a new transform for the gun.
    public void Zero(float radians = 0)
    {
        RotationNode.Rotation = new Vector3(0, 0, radians);
    }
}
