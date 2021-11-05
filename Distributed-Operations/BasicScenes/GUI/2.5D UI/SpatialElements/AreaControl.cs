using Godot;
using System;

public class AreaControl : Area
{
    [Export]
    float Thiccness;

    CollisionShape shape;
    public override void _Ready()
    {
        SpatialControl parent = (SpatialControl) GetParent();
        parent.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnSizeChanged),
            new Godot.Collections.Array {parent});

        shape = GetNode<CollisionShape>("CollisionShape");
    }

    public void OnSizeChanged( SpatialControl parent)
    {
        shape.Translation = new Vector3(parent.Size.x/2, -parent.Size.y/2, 0);
        ((BoxShape) shape.Shape).Extents = new Vector3(parent.Size.x/2, parent.Size.y/2, Thiccness);
    }
}
