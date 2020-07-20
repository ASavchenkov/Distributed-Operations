using Godot;
using System;


//Base class for the "receiver" of the gun.
//"Root" of the tree as far as mods go.
public abstract class Gun : Spatial
{
    SpawnManager ProjectileManager;

    //These need to be assigned for the gun to function correctly.
    protected Spatial ProjectileSpawn;
    protected IMunitionSource source;
    
    [Export]
    public float muzzleVelocity = 10;//In meters per second I think?

    public override void _Ready()
    {
        ProjectileManager = (SpawnManager) GetNode("/root/GameRoot/Projectiles");
    }

    //Default code for firing a projectile.
    //Unlikely anyone will need to modify this very much.
    public virtual void Fire()
    {
        
        string projectileScene = source.DequeueMunition();
        if(!(projectileScene is null))
        {
            Vector3 velocity = ProjectileSpawn.GlobalTransform.basis.Xform(-Vector3.Back) * muzzleVelocity;
            
            Projectile p = (Projectile) ProjectileManager.Spawn(projectileScene);
            p.Rpc("Init",ProjectileSpawn.GlobalTransform.origin, velocity);
        }

        
    }
    
    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(IsNetworkMaster())
        {
            if(inputEvent.IsActionPressed("ItemPrimary"))
            {
                Fire();
            }
        }
    }
// Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {

    // }
}
