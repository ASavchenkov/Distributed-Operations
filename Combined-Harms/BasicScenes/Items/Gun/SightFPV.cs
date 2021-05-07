using Godot;
using System;

public class SightFPV : Spatial
{
    //These may be the same node, or different ones
    //for ironsights "RotationNode" will probably be the front post
    // and "SightNode" will be the rear sight.


    //This is either the front post or the front plane of the optic.
    [Export]
    NodePath RotationNodePath;
    Spatial RotationNode;
    
    [Export]
    NodePath EyeReliefPath;
    RemoteTransform EyeRelief;
    public Position3D RemoteEyeRelief;


    public override void _Ready()
    {
        RotationNode = (Spatial) GetNode(RotationNodePath);
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
