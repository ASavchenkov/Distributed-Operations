using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class FileSystem : Control, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();

    public bool Permeable {get;set;} = true;
    
    private TwoFiveDCursor menu;
    private Folder rootFolder = null;
    
    [Export]
    string rootPath;

    public override void _Ready()
    {
        base._Ready();
        Claims.Claims.Add("ui_scroll_up");
        Claims.Claims.Add("ui_scroll_down");
        
        rootFolder = GetNode<Folder>("Folder");
        rootFolder.Path = rootPath;
        rootFolder.DispName = rootPath;
    }

    public override void _EnterTree()
    {
        //rootFolder might not be ready yet if this is the first opening.
        rootFolder?.Refresh();
    }


    //Don't actually care what happens on MouseOn/mouseOff
    public void MouseOn(TwoFiveDCursor menu)
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

    public interface IFSControl
    {
        //false means no longer exists and is going to be deleted.
        bool Refresh();
        string Path {get;set;}
    }
}