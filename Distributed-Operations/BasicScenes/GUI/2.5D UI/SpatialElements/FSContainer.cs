using Godot;
using System;

public class FSContainer : SpatialVBoxContainer
{
    Viewport viewport;
   
    public override void _Ready()
    {
        viewport = GetViewport();
        viewport.Connect("size_changed", this, nameof(OnSizeChanged));
    }

    public void OnSizeChanged()
    {
        var cam = viewport.GetCamera();
        var origin = cam.ProjectRayOrigin(new Vector2(0,viewport.Size.y));
        var 
        rect = viewport.Size;
    }
}
