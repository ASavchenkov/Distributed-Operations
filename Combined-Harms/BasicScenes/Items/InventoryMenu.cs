using Godot;
using System;

using ReplicationAbstractions;

public class InventoryMenu : Spatial
{

    public static NodeFactory<InventoryMenu> Factory
        = new NodeFactory<InventoryMenu>("res://BasicScenes/Items/InventoryMenu.tscn");
    
    ILootPV root = null;
    Spatial rootObserver;
    Camera cam;
    RayCast mouseRay;

    //Need to keep track of this for when we leave the mouseover.
    LootSlotObserver currentMouseOver = null;

    public void Subscribe(ILootPV _root)
    {
        root = _root;
        rootObserver = (Spatial) EasyInstancer.GenObserver( (Node) root, root.ObserverPathLootPV);
        
        //These should stay valid as it gets moved in and out of the tree.
        //So we do it here instead of _Ready()
        mouseRay = (RayCast) GetNode("MouseRay");
        GetNode("RootHandle").AddChild(rootObserver);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        var mouseMoveEvent = inputEvent as InputEventMouseMotion;
        if(!(mouseMoveEvent is null))
        {
            //Check the previous raycast
            //(since we can't do physics in input functions)
            //but that's fine since this function gets called multiple times
            //for any single mouse movement.
            PollRayCast();

            var to = cam.ProjectLocalRayNormal(mouseMoveEvent.Position);
            mouseRay.CastTo = to  * 10;

            GetTree().SetInputAsHandled();
        }
        else if(inputEvent.IsActionPressed("ItemPrimary"))
        {
            GD.Print("Clicky clicky lol");
            GetTree().SetInputAsHandled();
        }
    }

    private void PollRayCast()
    {
        LootSlotObserver pickedSlot = mouseRay.GetCollider() as LootSlotObserver;

        //The use of "null" as "not colliding with anything makes
        //null conditional operators very convenient.
        if(pickedSlot != currentMouseOver)
        {
            currentMouseOver?.MouseOff();
            currentMouseOver = pickedSlot;
            currentMouseOver?.MouseOn();
        }
    }

    public override void _EnterTree()
    {
        if(cam is null)
            cam = (Camera) GetParent();
        Input.SetMouseMode(Input.MouseMode.Visible);
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

}
