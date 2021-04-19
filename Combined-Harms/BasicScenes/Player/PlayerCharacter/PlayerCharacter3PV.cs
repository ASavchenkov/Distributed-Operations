using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacter3PV : Spatial, IObserver
{
    private PlayerCharacterProvider provider = null;
    private AnimationTree animTree;

    public override void _Ready()
    {
        animTree = (AnimationTree) GetNode("Soldier/AnimationTree");
        GD.Print(animTree.GetPropertyList());
    }
    
    public void Subscribe(Node provider)
    {
        this.provider = (PlayerCharacterProvider) provider;
        this.DefaultSubscribe(this.provider);
        provider.Connect(nameof(PlayerCharacterProvider.TrajectoryUpdated), this, nameof(OnTrajectoryUpdated));
    }

    public void OnTrajectoryUpdated(Vector3 translation, Vector3 velocity, Vector3 yaw, Vector3 pitch)
    {
        Translation = translation;
        Rotation = yaw;
        var localVelocity = GlobalTransform.Inverse().basis.Xform(velocity);
        animTree.Set("parameters/locomotion/blend_position", new Vector2(localVelocity.x, localVelocity.z));
        
        //Pitch will be used when we have IK working with the model.
    }
}
