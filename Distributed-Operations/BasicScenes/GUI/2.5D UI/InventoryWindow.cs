using Godot;
using System;

//Used to determine where dragged objects are shown and placed through raycast collision.
//Also moves all the loot stuff around by moving LootRoot Spatial.
public class InventoryWindow : ControlledBoxArea, IPickable
{
    public InputClaims Claims {get;set;} = new InputClaims();
    
    Spatial workspace;

    public bool Permeable {get;set;} = true;
    private TwoFiveDCursor menu = null;

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

    public void MouseOn(TwoFiveDCursor _menu)
    {
        menu = _menu;
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
            menu.Connect(nameof(TwoFiveDCursor.MouseUpdated), this, nameof(OnMouseUpdate));
            clickedOffset = ToLocal(menu.intersectionPoints[workspace]) - workspace.Translation;
            InputPriorityServer.Base.Subscribe(this, BaseRouter.dragging);
            trackMouse = true;
            return true;
        }
        else if (inputEvent.IsActionReleased("MouseSecondary") && trackMouse)
        {
            menu.Disconnect(nameof(TwoFiveDCursor.MouseUpdated), this, nameof(OnMouseUpdate));
            InputPriorityServer.Base.Unsubscribe(this, BaseRouter.dragging);
            trackMouse = false;
            return true;
        }
        return false;
    }
    public void OnMouseUpdate()
    {
        workspace.Translation = ToLocal(menu.intersectionPoints[workspace]) - clickedOffset;
    }

}
