using Godot;
using System;

public class InventoryWorkspace : Area, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();
    
    Spatial LootRoot;

    public bool Permeable {get;set;} = true;
    private TwoFiveDMenu menu = null;

    private Vector3 clickedOffset = new Vector3();
    private bool trackMouse = false;
    

    public override void _Ready()
    {
        LootRoot = GetNode<Spatial>("LootRoot");
        Claims.Claims.Add("MouseSecondary");
        base._Ready();
    }

    public void MouseOn(TwoFiveDMenu _menu)
    {
        menu = _menu;
        GD.Print("InventoryWorkspace MouseOn");
    }

    public void MouseOff()
    {
        if(trackMouse)
        {
            menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
            trackMouse = false;
        }
        GD.Print("InventoryWorkspace MouseOff");
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("MouseSecondary"))
        {
            menu.Connect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
            clickedOffset = ToLocal(menu.intersectionPoints[this]) - LootRoot.Translation;
            trackMouse = true;
            return true;
        }
        else if (inputEvent.IsActionReleased("MouseSecondary") && trackMouse)
        {
            menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
            trackMouse = false;
            return true;
        }
        return false;
    }
    public void OnMouseUpdate()
    {
        LootRoot.Translation = ToLocal(menu.intersectionPoints[this]) - clickedOffset;
    }

}
