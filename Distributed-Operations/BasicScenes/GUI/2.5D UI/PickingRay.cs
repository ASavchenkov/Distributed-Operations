using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using ReplicationAbstractions;

public class PickingRay : RayCast
{

    public static NodeFactory<PickingRay> Factory
        = new NodeFactory<PickingRay>("res://BasicScenes/GUI/2.5D UI/PickingRay.tscn");
    
    Camera cam;
    MultiRayCursor parent;
    // These should be in InventoryMenu
    //We're just using two raycasts, not two of this logic.
    
    const int pickingLimit = 10;

    public override void _Ready()
    {
        cam = GetViewport().GetCamera();
        parent = GetNode<UserObserver>("/root/UserObserver_1").Cursor;
    }

    public void UpdateCursor(Vector2 mousePos)
    {
        Translation = cam.ToLocal(cam.ProjectRayOrigin(mousePos));
        CastTo = cam.ProjectLocalRayNormal(mousePos) * 1e3f;
    }
}
