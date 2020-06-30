using Godot;
using System;

public class LocalGun : Gun
{

    PackedScene booletScene = (PackedScene) GD.Load("res://BasicScenes/Projectiles/Boolet/Boolet.tscn");
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    public void Fire()
    {
        GD.Print("Fire pressed");
        
        Vector3 velocity = BooletSpawn.GlobalTransform.basis.Xform(-Vector3.Back) * muzzleVelocity;
        Boolet boolet = (Boolet) booletScene.Instance();

        boolet.Init(velocity,BooletSpawn.GlobalTransform.origin);
        GameRoot.AddChild(boolet);
    }

    public void ADS()
    {

    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            Fire();
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
