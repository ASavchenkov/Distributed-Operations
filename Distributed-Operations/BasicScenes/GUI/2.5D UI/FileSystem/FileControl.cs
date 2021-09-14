using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class FileControl : Control, IPickable
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    
    FileInfo _FInfo;
    public FileInfo FInfo
    {
        get => _FInfo;
        set
        {
            _FInfo = value;
            Name = value.Name;
        }
    }
    
    Label label;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));

        label = GetNode<Label>("Name");
        label.Text = FInfo.Name;
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
        GD.Print("Dragged: ", FInfo.Name);
    }

}
