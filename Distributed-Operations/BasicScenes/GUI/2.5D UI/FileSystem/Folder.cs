using Godot;
using System;
using System.IO;

public class Folder : DraggableArea
{

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

    float width;
    bool showContents = false;

    public void Init(DirectoryInfo dirInfo, Vector3 loc, float width)
    {
        DirInfo = dirInfo;
        Translation = loc;
        this.width = width;
        GD.Print(Name, Translation);
    }

    public override void _Ready()
    {

    }

    public override void OnMouseUpdate()
    {
        //does nothing. I should really rethink this being an abstract.
    }
}
