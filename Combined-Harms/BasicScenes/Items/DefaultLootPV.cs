using Godot;
using System;

public class DefaultLootPV : PickableArea
{

    [Export]
    public float Radius {get;set;}

    public override void _Ready()
    {
        GD.Print(Name, " ready lol");
        
    }

    public virtual void FullClick()
    {

    }
    public override void SetGTransform(Transform globalTarget)
    {
        GlobalTransform = globalTarget;
    }
}