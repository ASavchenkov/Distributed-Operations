using Godot;
using System;
using System.IO;

public class FileSystem : ControlledBoxArea, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();

    public bool Permeable {get;set;} = true;
    
    private TwoFiveDMenu menu;

    [Export]
    float depth = 1;


    public static string savePath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "SaveData"
    );
    
    public override void _Ready()
    {
        Claims.Claims.Add("ui_scroll_up");
        Claims.Claims.Add("ui_scroll_down");

        base._Ready();
    }

    public void OntreeEntered()
    {
        DirectoryInfo saveDir = System.IO.Directory.CreateDirectory(savePath); //probably already exists
    }

    

    //Don't actually care what happens on MouseOn/mouseOff
    public void MouseOn(TwoFiveDMenu menu)
    {
        
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

