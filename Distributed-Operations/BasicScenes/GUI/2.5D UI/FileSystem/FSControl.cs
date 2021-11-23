using Godot;
using System;

using ReplicationAbstractions;

public class FSControl : SpatialControl, IAnchored, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();
    [Export]
    public AnchorMember anchorMember {get;set;}

    public bool Permeable {get;set;} = true;

    [Export]
    string RootPath = "res://Blueprints/";

    public override void _Ready()
    {
        anchorMember.Init(this);
        var rootFolder = FolderSpatial.Factory.Instance();
        rootFolder.Path = RootPath;
        rootFolder.DispName = RootPath;
        AddChild(rootFolder);
        base._Ready();
    }

    public void MouseOn( MultiRayCursor _cursor)
    {
        GD.Print("FSControl MouseOn");
    }
    public void MouseOff()
    {
        GD.Print("FSControl MouseOff");
    }

    public bool OnInput(InputEvent inputEvent) => false;
}
