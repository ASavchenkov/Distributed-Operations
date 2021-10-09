using Godot;
using System;

//Used to determine where dragged objects are shown and placed through raycast collision.
//Also moves all the loot stuff around by moving LootRoot Spatial.
public class InventoryWindow : ControlledBoxArea, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();
    
    Spatial workspace;

    public bool Permeable {get;set;} = true;
    private MultiRayCursor cursor = null;

    private Vector3 clickedOffset = new Vector3();
    private bool trackMouse = false;
    
    [Export]
    NodePath WorkspacePath;

    public override void _Ready()
    {
        workspace = GetNode<Spatial>(WorkspacePath);
        Claims.Claims.Add("MouseSecondary");
        base._Ready();
    }

    public void MouseOn(MultiRayCursor _cursor)
    {
        cursor = _cursor;
        GD.Print("InventoryWorkspace MouseOn");
    }

    public void MouseOff()
    {
        GD.Print("InventoryWorkspace MouseOff");
    }

    public bool OnInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("MouseSecondary"))
        {
            cursor.Connect(nameof(MultiRayCursor.CursorUpdated), this, nameof(OnCursorUpdate));
            clickedOffset = ToLocal(cursor.intersectionPoints[workspace]) - workspace.Translation;
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
        workspace.Translation = ToLocal(cursor.intersectionPoints[workspace]) - clickedOffset;
    }

}
