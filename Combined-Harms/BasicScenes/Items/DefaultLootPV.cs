using Godot;
using System;

using ReplicationAbstractions;

public class DefaultLootPV : Pickable, IObserver
{

    public virtual ILootItem provider {get; protected set;}
    public LootSlotObserver parent;
    
    [Signal]
    public delegate void RequestPosReset();

    [Export]
    public float Radius {get;set;}

    public override void _Ready()
    {
        GD.Print(Name, " ready lol");
    }

    public virtual void Subscribe( object _provider)
    {
        //in our case the provider is always a node
        //(until future refactor)
        provider = (ILootItem) _provider;
    }

    public virtual void FullClick()
    {

    }
    public override void SetGTransform(Transform globalTarget)
    {
        GlobalTransform = globalTarget;
    }

    public override void Release(TwoFiveDMenu menu)
    {
        base.Release(menu);
    }

}