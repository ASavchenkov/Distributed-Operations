using Godot;
using System;



//Items can be picked up and used.
public class Gun : Spatial
{
    protected Spatial GameRoot;
    protected Spatial BooletSpawn;
    PackedScene booletScene = (PackedScene) GD.Load("res://BasicScenes/Projectiles/Boolet/Boolet.tscn");
    
    [Export]
    public float muzzleVelocity = 10;//In meters per second I think?
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameRoot = (Spatial) GetTree().Root.GetNode("GameRoot");
        BooletSpawn = (Spatial) GetNode("BooletSpawn");
    }

    

    [PuppetSync]
    public void SpawnBoolet(Vector3 Translation, Vector3 LinearVelocity, int master)
    {
        Boolet boolet = (Boolet) booletScene.Instance();

        boolet.Init(BooletSpawn.GlobalTransform.origin, LinearVelocity);
        boolet.SetNetworkMaster(master);
        GameRoot.AddChild(boolet);
    }
    public virtual void Fire()
    {
        GD.Print("Fire pressed: ", GetTree().NetworkPeer.GetUniqueId());
        
        Vector3 velocity = BooletSpawn.GlobalTransform.basis.Xform(-Vector3.Back) * muzzleVelocity;
        Rpc("SpawnBoolet", BooletSpawn.GlobalTransform.origin, velocity, GetTree().NetworkPeer.GetUniqueId());
        
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
