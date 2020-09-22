using Godot;
using System;

public class SelectFireRifle : Gun
{

    Position3D HipFireTransform;

    public override void _Ready()
    {
        base._Ready();
        ProjectileSpawn = (Spatial) GetNode("Origin/Gun/Muzzle");
        source = (IMunitionSource) GetNode("Origin/Gun/Magazine");
        MainSight = (SightFPVObserver) GetNode("Origin/Gun/IronSights");
        HipFireTransform = (Position3D) GetNode("Origin/Gun/HipFireTransform");
        Origin.Transform = HipFireTransform.Transform.Inverse();
    }

    public override void Fire()
    {
        base.Fire();//Basic spawning should be the same. It's just the recoil we're worried about.
        EmitSignal("Recoil",0,0);
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
