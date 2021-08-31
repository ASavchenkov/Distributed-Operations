using Godot;
using System;

public class FileSystem : Area, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();

    public bool Permeable {get;set;} = true;
    private TwoFiveDMenu menu;
    public override void _Ready()
    {
        Claims.Claims.Add("ui_scroll_up");
        Claims.Claims.Add("ui_scroll_down");
    }

    public void MouseOn(TwoFiveDMenu menu)
    {
        this.menu = menu;
    }
    public void MouseOff()
    {

    }

    //Remember that this gets called when an event gets to us
    //if we're intersected by the mouse ray.
    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("ui_scroll_up"))
        {
            GD.Print("Scroll Up");
            return true;
        }
        else if (inputEvent.IsActionPressed("ui_scroll_down"))
        {
            GD.Print("Scroll Down");
            return true;
        }

        return false;
    }
}
