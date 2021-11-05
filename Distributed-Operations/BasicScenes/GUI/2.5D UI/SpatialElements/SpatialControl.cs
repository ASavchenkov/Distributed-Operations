using Godot;
using System;

//A control node but spatial so you can parent 3d stuff to it.
//And also so you can have good GUI in VR.
public class SpatialControl : Spatial
{
    private Vector2 _Size;
    [Export]
    public Vector2 Size
    {
        get => _Size;
        set
        {
            _Size = value;
            EmitSignal(nameof(SizeChanged));
        }
    }

    [Signal]
    public delegate void SizeChanged();

    [Export]
    float AnchorLeft = 0;
    [Export]
    float AnchorTop = 0;
    [Export]
    float AnchorRight = 1;
    [Export]
    float AnchorBottom = 1;

    [Export]
    float MarginLeft = 0;
    [Export]
    float MarginTop = 0;
    [Export]
    float MarginRight = 0;
    [Export]
    float MarginBottom = 0;

    public override void _Ready()
    {
        if(GetParent() is SpatialControl parent)
        {
            parent.Connect(nameof(SizeChanged), this, nameof(OnReferenceSizeChanged),
                new Godot.Collections.Array { parent });
        }
        base._Ready();
    }
    
    public virtual void OnReferenceSizeChanged(SpatialControl reference)
    {
        Translation = new Vector3(AnchorX(reference), AnchorY(reference), Translation.z);
        Size = new Vector2(AnchorSX(reference), AnchorSY(reference));
    }
    
    public float AnchorX(SpatialControl reference)
    {
        return AnchorLeft * reference.Size.x + MarginLeft;
    }
    public float AnchorY(SpatialControl reference)
    {
        return AnchorTop * reference.Size.y + MarginTop;
    }
    public float AnchorSX(SpatialControl reference)
    {
        return AnchorRight * reference.Size.x + MarginRight - Translation.x;
    }
    public float AnchorSY(SpatialControl reference)
    {
        return AnchorBottom * reference.Size.y + MarginRight - Translation.y;
    }
}
