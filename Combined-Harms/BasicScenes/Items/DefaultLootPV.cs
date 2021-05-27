using Godot;
using System;

public class DefaultLootPV : Area, ILootPickable
{

    private PickingMember _pMember;
    public PickingMember pMember{get => _pMember;}
    [Export]
    public float Radius {get;set;}

    public override void _Ready()
    {
        GD.Print("ready lol");
        _pMember = new PickingMember(this, this);
        
    }

    public virtual void FullClick()
    {

    }
    public void UpdateTransform(Transform globalTarget)
    {
        GlobalTransform = globalTarget;
    }
}