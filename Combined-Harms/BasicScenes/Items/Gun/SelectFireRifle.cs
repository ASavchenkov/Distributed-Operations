using Godot;
using System;

public class SelectFireRifle : Gun
{
    public override void _Ready()
    {
        base._Ready();
        ProjectileSpawn = (Spatial) GetNode("Muzzle");
        source = (IMunitionSource) GetNode("Magazine");
    }
}
