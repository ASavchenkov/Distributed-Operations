using Godot;
using System;
using System.IO;
using System.Collections.Generic;

using ReplicationAbstractions;

public class Folder : Control, IPickable, FileSystem.IFSControl//, IAcceptsItem
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");

    public Godot.Directory dir = new Godot.Directory();
    
    string _Path;
    public string Path
    {
        get => _Path;
        set
        {
            _Path = value;
            dir.Open(value);
            Refresh();
        }
    }
    string _DispName;
    public string DispName
    {
        get => _DispName;
        set
        {
            _DispName = value;
            if(!(label is null))
                label.Text = value;
        }
    }
    Label label;

    bool showContents = false;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.FullClick), this, nameof(OnClick));

        dir.Open(Path);
        label = GetNode<Label>("HSplitContainer/Name");
        label.Text = DispName;
    }

    public void MouseOn(TwoFiveDCursor menu)
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
            showContents = true;
            Refresh();
        }
        
    }
    
    //Recursive (calls refresh on all of it's children too.)
    public bool Refresh()
    {

        //if we no longer exist, stop showing ourself (and everyone under us.)
        if(!dir.DirExists(Path))
        {
            QueueFree();
            return false;
        }
        //End it here if we're not currently showing contents.
        if(!showContents)
            return true;
        
        //Otherwise we refresh everything under us.
        HashSet<string> existingPaths = new HashSet<string>();
        var contentContainer = GetNode("Contents/Contents");
        foreach (Node existing in contentContainer.GetChildren())
        {
            var ctrl = existing as FileSystem.IFSControl;
            if(!(ctrl is null) && ctrl.Refresh())
            {
                existingPaths.Add(ctrl.Path);
            }
        }
        dir.ListDirBegin(skipNavigational: true);
        string child = dir.GetNext();
        while(child != "")
        {
            var childPath = Path + child;
            if(!existingPaths.Contains(childPath))
            {
                if(dir.DirExists(child))
                {
                    var subFolder = EasyInstancer.Instance<Folder>("res://BasicScenes/GUI/2.5D UI/FileSystem/Folder.tscn");
                    subFolder.Path = childPath + "/";
                    subFolder.DispName = child;
                    contentContainer.AddChild(subFolder);
                }
                else if (dir.FileExists(child))
                {
                    // var filee = new Godot.File();
                    // GD.Print(filee.FileExists(dir.GetCurrentDir() + child));
                    // GD.Print("Added file of name: ", child, ", Path: ", childPath);
                    var file = EasyInstancer.Instance<FileControl>("res://BasicScenes/GUI/2.5D UI/FileSystem/FileControl.tscn");
                    file.Path = childPath;
                    file.DispName = child;
                    contentContainer.AddChild(file);
                }
            }
            child = dir.GetNext();
        }
        return true;
    }

    // public bool AcceptItem( DefaultInvPV item)
    // {
    //     if(file.Open(Path, Godot.File.ModeFlags.Write) != Error.Ok)
    //     {
    //         GD.PrintErr("can't open file bruh");
    //         return;
    //     }
    //     string serialized = MessagePackSerializer.SerializeToJson<SerializedNode>(target);
    //     GD.Print(serialized);
    //     file.StoreString(serialized.PrettyPrintJson());
    //     file.Close();
    //     return true;
    // }
}