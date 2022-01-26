using Godot;
using System;

public class AreaControl : Area
{
    [Export]
    float Thiccness = 0.05f;
    
    CollisionShape shape;
    public override void _Ready()
    {
        SpatialControl parent = (SpatialControl) GetParent();
        parent.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnSizeChanged),
            new Godot.Collections.Array {parent});

        shape = GetNode<CollisionShape>("CollisionShape");
    }

    public void OnSizeChanged( Vector2 oldSize, SpatialControl parent)
    {
        shape.Translation = new Vector3(parent.Size.x/2, -parent.Size.y/2, 0);
        ((BoxShape) shape.Shape).Extents = new Vector3(parent.Size.x/2, parent.Size.y/2, Thiccness);
    }
}
