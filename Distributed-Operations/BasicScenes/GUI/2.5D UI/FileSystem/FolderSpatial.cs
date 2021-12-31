using Godot;
using System;
using System.IO;
using System.Collections.Generic;

using MessagePack;
using JsonPrettyPrinterPlus;

using ReplicationAbstractions;

public class FolderSpatial : SpatialControl, IPickable, IAnchored, IAcceptsItem 
{

    public static NodeFactory<FolderSpatial> Factory
         = new NodeFactory<FolderSpatial>("res://BasicScenes/GUI/2.5D UI/FileSystem/FolderSpatial.tscn");

    public bool Permeable {get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    [Export]
    public AnchorMember anchorMember {get;set;}

    protected MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    protected MultiRayCursor cursor = null;
    
    SpatialLabel label;
    public Godot.Directory dir = new Godot.Directory();
    
    string _Path;
    [Export]
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
            Name = value;
            _DispName = value;
            if(!(label is null))
                label.Text = value;
        }
    }
    bool showContents = false;

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    public override void _Ready()
    {
        anchorMember.Init(this);

        label = GetNode<SpatialLabel>("Label");
        label.Size = new Vector2(label.Size.x, 0.36f);
        label.Text = DispName;

        Claims = M1.Claims;// Just link to M1 for now since it's the only one.
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        M1.Connect(nameof(MouseActionTracker.FullClick), this, nameof(OnClick));
        base._Ready();
    }

    public virtual void MouseOn(MultiRayCursor _cursor)
    {
        GD.Print(Name, ": Moused on");
        cursor = _cursor;
        M1.cursor = cursor;
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff()
    {
        GD.Print(Name, ": Moused off");   
    }

    public virtual void OnDrag()
    {
        GD.Print("OnDrag");
        if(dir.DirExists("."))
            GD.Print("This directory exists: ", Path);
        
    }

    public virtual bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }

    //Open/Close the folder
    public void OnClick()
    {
        GD.Print("folder onclick");
        if(showContents)
        {
            GetNode("Contents").QueueFree();
            showContents = false;
        }
        else
        {
            dir.ListDirBegin(skipNavigational: true);

            //Do nothing if no one is under us.
            string next = dir.GetNext();
            if(next =="")
                return;
            else
            {
                dir.ListDirEnd();
                showContents = true;
                var contents = EasyInstancer.Instance<SpatialVBoxContainer>("res://BasicScenes/GUI/2.5D UI/FileSystem/FolderContents.tscn");
                AddChild(contents);
                contents.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnContentSizeChange),
                    new Godot.Collections.Array {contents});
                Refresh();
            }
        }
    }

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

        var contents = GetNode<SpatialVBoxContainer>("Contents");
        foreach (Node existing in contents.GetChildren())
        {
            var ctrl = existing as IFSControl;
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
                    var subFolder = Factory.Instance();
                    subFolder.Path = childPath + "/";
                    //lazy way to indicate this is a folder.
                    //Will replace with better one hopefully.
                    subFolder.DispName = child + "/";
                    contents.AddSpatialControl(subFolder);
                }
                else if (dir.FileExists(child))
                {
                    // var filee = new Godot.File();
                    // GD.Print(filee.FileExists(dir.GetCurrentDir() + child));
                    // GD.Print("Added file of name: ", child, ", Path: ", childPath);
                    var file = EasyInstancer.Instance<FileSpatial>("res://BasicScenes/GUI/2.5D UI/FileSystem/FileSpatial.tscn");
                    file.Path = childPath;
                    file.DispName = child;
                    contents.AddSpatialControl(file);
                }
            }
            child = dir.GetNext();
        }
        return true;
    }

    public void OnContentSizeChange(Vector2 oldSize, SpatialControl contents)
    {
        if( Math.Abs(oldSize.y - contents.Size.y) < 1e-7)
            return;
        Size = new Vector2( Size.x, 0.36f + contents.Size.y);
        GD.Print(Name, ": ContentSize was: ", contents.Size);
    }
    public bool AcceptItem( DefaultInvPV item)
    {
        //Should just be the GUID
        string name = item.Provider.Name;
        Godot.File file = new Godot.File();
        //Needs to be a new file.
        if(file.FileExists(Path + "/" + name))
        {
            GD.PrintErr("file: ", name, " already exists");
            return false;
        }
        Error openErr = file.Open(Path + name, Godot.File.ModeFlags.Write);
        if( openErr != Error.Ok)
        {
            GD.PrintErr("can't open file due to error: ", openErr);
            return false;
        }

        string serialized = new SerializedNode(item.Provider).AsJson();
        GD.Print(serialized);
        file.StoreString(serialized.PrettyPrintJson());
        file.Close();
        return true;
    }

}

public interface IFSControl
{
    //false means no longer exists and is going to be deleted.
    bool Refresh();
    string Path {get;set;}
}