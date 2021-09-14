using Godot;
using System;
using System.IO;

using ReplicationAbstractions;

public class Folder : Control, IPickable
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

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
        label = GetNode<Label>("HSplitContainer/Name");
        label.Text = DirInfo.Name;

        var contentContainer = GetNode("Contents/Contents");
        DirectoryInfo[] subdirs = DirInfo.GetDirectories();
        foreach( DirectoryInfo i in subdirs)
        {
            var subFolder = EasyInstancer.Instance<Folder>("res://BasicScenes/GUI/2.5D UI/FileSystem/Folder.tscn");
            subFolder.DirInfo = i;
            contentContainer.AddChild(subFolder);
        }
        
    }

    public void MouseOn(TwoFiveDMenu menu)
    {

    }
    public void MouseOff()
    {

    }
    public bool OnInput(InputEvent inputEvent)
    {
        return false;
    }
}
