using Godot;
using System;

//Used to determine where dragged objects are shown and placed through raycast collision.
//Also moves all the loot stuff around by moving LootRoot Spatial.
public class InventoryWindow : SpatialControl, IAnchored, ITakesInput
{
    public InputClaims Claims {get;set;} = new InputClaims();

    [Export]
    public AnchorMember anchorMember {get;set;}

    private PickableAreaControl aCtrl;
    private MultiRayCursor cursor = null;

    private Vector3 clickedOffset = new Vector3();
    private bool trackMouse = false;
    
    [Export]
    NodePath WorkspacePath;

    Spatial workspace;

    public override void _Ready()
    {
        base._Ready();

        anchorMember.Init(this);
        workspace = GetNode<Spatial>(WorkspacePath);
        Claims.Claims.Add("MouseSecondary");

        aCtrl = GetNode<PickableAreaControl>("AreaControl");
        aCtrl.PickingMember = new PickingMixin(this, true, nameof(MouseOn), nameof(MouseOff));
    }

    public void MouseOn(MultiRayCursor _cursor)
    {
        cursor = _cursor;
        GD.Print("InventoryWindow MouseOn");
    }

    public void MouseOff()
    {
        GD.Print("InventoryWindow MouseOff");
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("MouseSecondary"))
        {
            cursor.Connect(nameof(MultiRayCursor.CursorUpdated), this, nameof(OnCursorUpdate));
            clickedOffset = ToLocal(cursor.intersectionPoints[(IPickable)workspace]) - workspace.Translation;
            InputPriorityServer.Base.Subscribe(this, BaseRouter.dragging);
            trackMouse = true;
            return true;
        }
        else if (inputEvent.IsActionReleased("MouseSecondary") && trackMouse)
        {
            cursor.Disconnect(nameof(MultiRayCursor.CursorUpdated), this, nameof(OnCursorUpdate));
            InputPriorityServer.Base.Unsubscribe(this, BaseRouter.dragging);
            trackMouse = false;
            return true;
        }
        return false;
    }
    public void OnCursorUpdate()
    {
        workspace.Translation = ToLocal(cursor.intersectionPoints[(IPickable)workspace]) - clickedOffset;
    }

}
