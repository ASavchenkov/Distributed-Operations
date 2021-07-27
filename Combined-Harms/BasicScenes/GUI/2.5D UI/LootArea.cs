using Godot;
using System;

public class LootArea : Area, ITakesInput, ITakesMouseRay, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();
    
    Spatial LootRoot;

    public bool Permeable {get;set;} = true;

    public override void _Ready()
    {
        LootRoot = GetNode<Spatial>("LootRoot");
        Claims.Claims.Add("MouseSecondary");
        base._Ready();
    }

    public void MouseOn(TwoFiveDMenu _menu)
    {
        GD.Print("LootArea MouseOn");
    }

    public void MouseOff(TwoFiveDMenu _menu)
    {
        GD.Print("LootArea MouseOff");
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsAction("MouseSecondary"))
            return true;
        return false;
    }

    public void CollideMouseRay(Vector3 collisionPoint)
    {
        
    }
}
