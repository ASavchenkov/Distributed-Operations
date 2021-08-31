using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleFPV : Spatial, ITakesInput, IObserver
{

    RifleProvider provider;

    public InputClaims Claims {get;set;} = new InputClaims();

    private Node Projectiles;
    protected Spatial Muzzle;
    protected IMunitionSource source;
    protected Spatial Origin;
    protected Position3D HipFireTransform;
    protected SightFPV MainSight;
    //When we start to do things like canted sights
    //we will need to revisit this.

    [Export]
    public float muzzleVelocity = 10;//In meters per second I think?

    //camera recoil effects can really only happen in the x and y directions,
    //so we don't worry about using full transforms.
    // (Though we should experiment with z recoil.)
    [Signal]
    public delegate void Recoil(float x, float y);

    [Export]
    Vector3 GlobalMuzzle;
    
    public override void _Ready()
    {
        Origin = (Spatial) GetNode("Origin");
        Projectiles = GetNode("/root/GameRoot/Projectiles");
        MainSight = (SightFPV) GetNode("Origin/Gun/FrontPostRail/IronSights");
        HipFireTransform = (Position3D) GetNode("Origin/Gun/HipFireTransform");
        Muzzle = (Spatial) GetNode("Origin/Gun/Muzzle");
        
        Claims.Claims.Add("MousePrimary");
        Claims.Claims.Add("MouseSecondary");

        SetOrigin(HipFireTransform);
    }

    public void Subscribe(object _provider)
    {
        this.DefaultSubscribe((Node) _provider);
        provider = (RifleProvider) _provider;
        source = provider.Mag;
    }

    //Default code for firing a projectile.
    //Unlikely anyone will need to modify this very much.
    //But you can!
    public virtual void Fire()
    {
        string projectileScene = source.DequeueMunition();
        if(!(projectileScene is null))
        {
            Vector3 velocity = Muzzle.GlobalTransform.basis.Xform(-Vector3.Left) * muzzleVelocity;
            
            ProjectileProvider p = EasyInstancer.Instance<ProjectileProvider>(projectileScene);
            Projectiles.AddChild(p);
            p.Rpc("Init",Muzzle.GlobalTransform.origin, velocity);
        }
    }

    public void SetOrigin(Spatial newOrigin)
    {
        Origin.Transform = newOrigin.Transform.Inverse();
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("MousePrimary"))
        {
            Fire();
            return true;
        }
        else if(inputEvent.IsActionPressed("MouseSecondary"))
        {
            SetOrigin(MainSight.RemoteEyeRelief);
            return true;
        }
        else if(inputEvent.IsActionReleased("MouseSecondary"))
        {
            SetOrigin(HipFireTransform);
            return true;
        }
        return false;
    }
}
