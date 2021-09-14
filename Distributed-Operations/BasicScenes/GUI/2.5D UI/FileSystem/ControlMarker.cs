using Godot;
using System;

//C# implementation of: https://github.com/godotengine/godot-demo-projects/blob/master/3d/waypoints
public class ControlMarker : Position3D
{
    Camera camera;
    public override void _Ready()
    {
        camera = GetViewport().GetCamera();
    }
    
    
    public override void _Process(float delta)
    {
        if(!camera.Current)
            camera = GetViewport().GetCamera();
        var camPos = camera.GlobalTransform.origin;
        var markerPos = GlobalTransform.origin;
    }
}
