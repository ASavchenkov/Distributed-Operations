using Godot;
using System;

public class PlayerCharacter3PV : Spatial, IObserver<PlayerCharacterProvider>
{
    private PlayerCharacterProvider provider = null;

    public void Init(PlayerCharacterProvider provider)
    {
        this.provider = provider;
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
