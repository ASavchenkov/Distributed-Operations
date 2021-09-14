using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class Folder : Control, IPickable
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");

    DirectoryInfo _DirInfo;
    public DirectoryInfo DirInfo
    {
        get => _DirInfo;
        set
        {
            _DirInfo = value;
            Name = value.Name;
        }
    }

    Label label;

    bool showContents = false;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.FullClick), this, nameof(OnClick));

        label = GetNode<Label>("HSplitContainer/Name");
        label.Text = DirInfo.Name;
    }

    public void MouseOn(TwoFiveDMenu menu)
    {
        M1.menu = menu;
    }
    public void MouseOff()
    {

    }
    public bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }
    public void OnClick()
    {
        if(showContents)
        {
            var contentContainer = GetNode("Contents/Contents");
            foreach(Node child in contentContainer.GetChildren())
                child.QueueFree();
            showContents = false;
        }
        else
        {
            loadChilren();
        }
        
    }

    private void loadChilren()
    {
        var contentContainer = GetNode("Contents/Contents");
        DirectoryInfo[] subdirs = DirInfo.GetDirectories();
        foreach( DirectoryInfo i in subdirs)
        {
            var subFolder = EasyInstancer.Instance<Folder>("res://BasicScenes/GUI/2.5D UI/FileSystem/Folder.tscn");
            subFolder.DirInfo = i;
            contentContainer.AddChild(subFolder);
        }
        showContents = true;
    }
}
