using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Timers;

using MessagePack;
using JsonPrettyPrinterPlus;

using ReplicationAbstractions;

public class FolderSpatial : SpatialControl, ITakesInput, IAnchored, IAcceptsItem, IFSControl
{

    public static NodeFactory<FolderSpatial> Factory
         = new NodeFactory<FolderSpatial>("res://BasicScenes/GUI/2.5D UI/FileSystem/FolderSpatial.tscn");

    public InputClaims Claims {get;set;} = new InputClaims();
    PickableAreaControl LabelArea;

    [Export]
    public AnchorMember anchorMember {get;set;}

    protected MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    protected DoubleClickTracker DoubleClickM1 = new DoubleClickTracker();
    protected MultiRayCursor cursor = null;
    System.Timers.Timer pollingLimiter = new System.Timers.Timer(100);
    
    SpatialLabel label;
    public Godot.Directory dir = new Godot.Directory();


    public bool Exists {get => dir.DirExists(".");}
    public string Path {get => dir.GetCurrentDir();}


    public bool HasChildren {get; private set;} = false;

    List<FolderSpatial> CachedSubfolders = new List<FolderSpatial>();


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
    
    public override void _Ready()
    {
        
        base._Ready();
        anchorMember.Init(this);
        pollingLimiter.Enabled = true;
        
        label = GetNode<SpatialLabel>("Label");
        label.Text = DispName;

        LabelArea = GetNode<PickableAreaControl>("Label/AreaControl");
        LabelArea.PickingMember = new PickingMixin(this, false, nameof(MouseOn), nameof(MouseOff));

        Claims = M1.Claims;// Just link to M1 for now since it's the only one.
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        M1.Connect(nameof(MouseActionTracker.FullClick), DoubleClickM1, nameof(DoubleClickTracker.OnClick));
        
        RefreshChildren();
    }

    //Remember that Godot.Directory.Open only takes full paths.
    public void Open(string path)
    {
        dir.Open(path);
        RefreshChildren();
    }

    //Equivalent to mv command in linux.
    //No point in always having to type "." in rename command
    //when we have an object with the path already in it.
    //Might end up removing the whole folder and adding it to another folder.
    public void Rename(string newPath) => dir.Rename(".", newPath);
    

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
    public void OnButtonClick( bool newState)
    {
        GD.Print("folder onclick");
        if(newState)
        {
            var contents = EasyInstancer.Instance<FolderContents>("res://BasicScenes/GUI/2.5D UI/FileSystem/FolderContents.tscn");
            contents.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnContentSizeChange),
                    new Godot.Collections.Array {contents});
            AddChild(contents);
            
            foreach(FolderSpatial subfolder in CachedSubfolders)
            {
                if(subfolder.Exists)
                    contents.AddChild(subfolder);
                else
                    subfolder.Free();
            }
            CachedSubfolders.Clear();
            
            RefreshChildren();
        }
        else
        {
            foreach( Node child in GetNode("Contents").GetChildren())
            {
                if(child is FolderSpatial f && f.HasNode("Contents"))
                    CachedSubfolders.Add(f);
            }
            GetNode("Contents").QueueFree();
            Size = new Vector2(Size.x, label.Size.y);
        }
    }

    public void RefreshChildren()
    {
        
        dir.ListDirBegin(skipNavigational: true);
        string child  = dir.GetNext();

        var contents = GetNodeOrNull<SpatialVBoxContainer>("Contents");
        var button = GetNodeOrNull("FolderButton");
        
        //folder is empty.
        if(child == "")
        {
            contents?.QueueFree();
            button?.QueueFree();
            return;
        }

        //Since there are children, but no button, add a button and return.
        if(button is null)
        {
            button = EasyInstancer.Instance<Node>("res://BasicScenes/GUI/2.5D UI/FileSystem/FolderButton.tscn");
            button.Connect(nameof(FolderButton.Clicked), this, nameof(OnButtonClick));
            AddChild(button);
            return;
        }

        //check if we're displaying the children.
        if(contents is null)
        {
            //if we are, remove the ones that no longer exist.
            //(perhaps they wer deleted outside the game.)
            CachedSubfolders.RemoveAll( x =>
            {
                if(!x.Exists)
                {
                    x.Free();
                    return true;
                }
                else
                return false;
            });
            return;
        }

        HashSet<string> existingPaths = new HashSet<string>();

        foreach (Node existing in contents.GetChildren())
        {
            var ctrl = existing as IFSControl;
            if(ctrl is null)
                continue;

            if(!ctrl.Exists)
                ((Node)ctrl).QueueFree();
            else
                existingPaths.Add(ctrl.Path);
            
        }
        
        //the listdir has already started
        //and child already definitely has a real string in it.
        while(child != "")
        {
            var childPath = Path + child;
            if(!existingPaths.Contains(childPath))
            {
                if(dir.DirExists(child))
                {
                    var subFolder = Factory.Instance();
                    subFolder.Open(childPath);
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
    }


    public void OnContentSizeChange(Vector2 oldSize, SpatialControl contents)
    {
        if( Math.Abs(oldSize.y - contents.Size.y) < 1e-7)
            return;
        Size = new Vector2( Size.x, contents.Translation.y + contents.Size.y);
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
    //Doesn't cover contents. This is handled separately for folders.
    bool Exists {get;}
    string Path {get;}
}