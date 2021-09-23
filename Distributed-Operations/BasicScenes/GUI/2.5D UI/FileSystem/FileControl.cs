using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class FileControl : Control, IPickable, FileSystem.IFSControl
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    
    public Godot.File file = new Godot.File();
    public string Path {get;set;}
    public string DispName;
    Label label;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        
        label = GetNode<Label>("Name");
        label.Text = DispName;
    }

    public bool Refresh()
    {
        if(!file.FileExists(Path))
        {
            QueueFree();
            return false;
        }
        return true;
    }
    public void MouseOn(TwoFiveDMenu menu)
    {
        M1.menu = menu;
    }
    
    public void MouseOff() {}
    
    public bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }

    public void OnDrag()
    {
        GD.Print("Dragged: ", Name);
    }

}
