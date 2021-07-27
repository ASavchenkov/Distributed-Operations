using Godot;
using System;

public class LootArea : Area, ITakesInput, IPickable
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
        InputPriorityServer.Base.Subscribe(this, BaseRouter.mouseOver);
        GD.Print("LootArea MouseOn");
    }

    public void MouseOff()
    {
        InputPriorityServer.Base.Unsubscribe(this, BaseRouter.mouseOver);
        if(trackMouse)
        {
            menu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
            trackMouse = false;
        }
        GD.Print("LootArea MouseOff");
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("MouseSecondary"))
        {
            menu.Connect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(OnMouseUpdate));
            clickedOffset = ToLocal(menu.mouseIntersections[this]) - LootRoot.Translation;
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
        LootRoot.Translation = ToLocal(menu.mouseIntersections[this]) - clickedOffset;
    }

}
