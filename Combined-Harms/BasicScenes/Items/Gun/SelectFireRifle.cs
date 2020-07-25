using Godot;
using System;

public class SelectFireRifle : Gun
{

    Position3D HipFireTransform;

    public override void _Ready()
    {
        base._Ready();
        ProjectileSpawn = (Spatial) GetNode("Origin/Muzzle");
        source = (IMunitionSource) GetNode("Origin/Magazine");
        MainSight = (Sight) GetNode("Origin/IronSights");
        HipFireTransform = (Position3D) GetNode("Origin/HipFireTransform");
        Origin.Transform = HipFireTransform.Transform.Inverse();
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        base._UnhandledInput(inputEvent);
        if(IsNetworkMaster())
        {
            if(inputEvent.IsActionPressed("ItemSecondary"))
            {
                SetOriginToSight(MainSight);
            }
            else if(inputEvent.IsActionReleased("ItemSecondary"))
            {
                Origin.Transform = HipFireTransform.Transform.Inverse();
            }
        }
    }
}
