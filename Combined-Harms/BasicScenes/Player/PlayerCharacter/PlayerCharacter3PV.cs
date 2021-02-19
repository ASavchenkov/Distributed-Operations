using Godot;
using System;

using ReplicationAbstractions;

public class PlayerCharacter3PV : Spatial, IObserver
{
    private PlayerCharacterProvider provider = null;

    public void Subscribe(Node provider)
    {
        this.provider = (PlayerCharacterProvider) provider;
        provider.Connect("tree_exiting", this, "queue_free");
        provider.Connect(nameof(PlayerCharacterProvider.TrajectoryUpdated), this, nameof(OnTrajectoryUpdated));
    }

    public void OnTrajectoryUpdated(Vector3 translation, Vector3 yaw, Vector3 pitch)
    {
        Translation = translation;
        Rotation = yaw;
        //Pitch will be used when we have IK working with the model.
    }
}
