using Godot;
using System;

//Only works with Orthogonal camera at the moment.
public class ViewportFillContainer : SpatialControl
{

    public override void _Ready()
    {
        GetViewport().Connect("size_changed",this, nameof(OnViewportSize));
        OnViewportSize();
        base._Ready();
    }

    public void OnViewportSize()
    {
        var cam = (Camera) GetParent();
        if(cam.Current)
        {
            Vector2 size = GetViewport().Size;
            float aspectRatio = size.x/size.y;
            float width;
            float height;
            if(cam.KeepAspect == Camera.KeepAspectEnum.Height)
            {
                height = cam.Size;
                width = height * aspectRatio;
            }
            else
            {
                width = cam.Size;
                height = width * aspectRatio;
            }

            Translation = new Vector3(-width/2, height/2, Translation.z);
            Size = new Vector2(width, height);
        }
    }
}
