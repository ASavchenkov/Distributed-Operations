using Godot;
using System;

public class LootArea : Area, IPickable
{
    TwoFiveDMenu attachedMenu;
    Transform offset;
    public bool Permeable {get;set;} = true;

    public void MouseOn(TwoFiveDMenu _menu)
    {
        GD.Print("LootArea MouseOn");
    }

    public void MouseOff(TwoFiveDMenu _menu)
    {
        GD.Print("LootArea MouseOff");
    }
}
