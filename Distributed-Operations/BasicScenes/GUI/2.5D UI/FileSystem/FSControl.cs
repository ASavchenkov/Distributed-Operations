using Godot;
using System;

using ReplicationAbstractions;

public class FSControl : SpatialControl, IAnchored, ITakesInput
{
    [Export]
    public AnchorMember anchorMember {get;set;}

    public InputClaims Claims {get;set;} = new InputClaims();
    private PickableAreaControl aCtrl;

    [Export]
    string RootPath = "res://Blueprints/";

    public override void _Ready()
    {
        base._Ready();
        anchorMember.Init(this);
        aCtrl = GetNode<PickableAreaControl>("AreaControl");
        aCtrl.PickingMember = new PickingMixin(this, true, nameof(MouseOn), nameof(MouseOff));
        
        var rootFolder = FolderSpatial.Factory.Instance();
        rootFolder.Open(RootPath);
        rootFolder.DispName = RootPath;
        AddChild(rootFolder);
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
