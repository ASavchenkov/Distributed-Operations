using Godot;
using System;
using System.Collections.Generic;

using ReplicationAbstractions;

public class RifleFPV : Spatial, IObserver
{
    
    [Export]
    Dictionary<string,NodePath> attachmentMap;

    RifleProvider provider;

    private Node Projectiles;
    protected Spatial Muzzle;
    protected IMunitionSource source;
    protected Spatial Origin;
    protected Position3D HipFireTransform;
    protected SightFPVObserver MainSight;
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
        MainSight = (SightFPVObserver) GetNode("Origin/Gun/FrontPostRail/IronSights");
        HipFireTransform = (Position3D) GetNode("Origin/Gun/HipFireTransform");
        Muzzle = (Spatial) GetNode("Origin/Gun/Muzzle");
    }

    public void Subscribe(Node provider)
    {
        this.DefaultSubscribe(provider);
        this.provider = (RifleProvider) provider;
        this.provider.Connect(nameof(RifleProvider.AttachmentUpdated), this, nameof(OnAttachmentUpdated));
        source = this.provider.Mag;
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
            p.SetNetworkMaster(GetTree().GetNetworkUniqueId());
            Projectiles.AddChild(p);
            p.Rpc("Init",Muzzle.GlobalTransform.origin, velocity);
        }
    }

    public void OnAttachmentUpdated(string attachPoint, IFPV attachment)
    {
        Node parentNode = GetNode(attachmentMap[attachPoint]);
        for(int i = 0; i<parentNode.GetChildCount(); i++)
        {
            parentNode.GetChild(i).QueueFree();
        }
        //If null was passed, there is no attachment.
        if(!(attachment is null))
        {
            var observer = EasyInstancer.Instance<Node>(attachment.ObserverPathFPV);
            parentNode.AddChild(EasyInstancer.GenObserver((Node) attachment, attachment.ObserverPathFPV));
        }
    }

    public void SetOriginToSight(SightFPVObserver sight)
    {
        Origin.Transform = sight.RemoteEyeRelief.Transform.Inverse();
        //this will also naturally update the sights global transform.
    }
    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            Fire();
        }
        else if(inputEvent.IsActionPressed("ItemSecondary"))
        {
            SetOriginToSight(MainSight);
        }
        else if(inputEvent.IsActionReleased("ItemSecondary"))
        {
            Origin.Transform = HipFireTransform.Transform.Inverse();
        }
    }
}
