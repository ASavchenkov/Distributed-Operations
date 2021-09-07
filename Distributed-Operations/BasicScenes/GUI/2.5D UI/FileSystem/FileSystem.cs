using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class FileSystem : ControlledBoxArea, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();

    public bool Permeable {get;set;} = true;
    
    private TwoFiveDMenu menu;
    private Folder rootFolder = null;
    
    public override void _Ready()
    {
        base._Ready();
        Claims.Claims.Add("ui_scroll_up");
        Claims.Claims.Add("ui_scroll_down");
        Connect("tree_entered",this, nameof(OntreeEntered));
        OntreeEntered();
        Connect("tree_exiting",this, nameof(OnTreeExiting));
    }

    public void OntreeEntered()
    {
        DirectoryInfo saveDir = new DirectoryInfo(Godot.OS.GetUserDataDir() + "/Blueprints");
        rootFolder = EasyInstancer.Instance<Folder>("res://BasicScenes/GUI/2.5D UI/FileSystem/Folder.tscn");
        rootFolder.Init(saveDir, tracker.Size, tracker.Size.x);
        AddChild(rootFolder);
        
    }

    public void OnTreeExiting()
    {
        rootFolder.QueueFree();
        rootFolder = null;
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

