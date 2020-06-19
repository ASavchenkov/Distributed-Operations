using Godot;
using System;



//Items can be picked up and used.
public class Gun : Spatial
{
    Spatial GameRoot;
    Spatial BooletSpawn;
    PackedScene booletScene = (PackedScene) GD.Load("res://BasicScenes/Items/Boolet/Boolet.tscn");
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameRoot = (Spatial) GetTree().Root.GetNode("GameRoot");
        BooletSpawn = (Spatial) GetNode("BooletSpawn");
        
    }

    public void Fire()
    {
        GD.Print("Fire pressed");
        Vector3 direction = BooletSpawn.GlobalTransform.basis.Xform(-Vector3.Back);
        Boolet boolet = (Boolet) booletScene.Instance();

        boolet.Init(direction,BooletSpawn.GlobalTransform.origin);
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
    public override void _Process(float delta)
    {

    }
}
