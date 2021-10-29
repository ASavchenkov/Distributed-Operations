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
    bool ProportionControl = false;
    [Export]
    public Vector2 RelativePosition; //(0,0) is top left, (1,-1) is bottom right.
    [Export]
    public Vector2 RelativeSize;

    public override void _Ready()
    {
        if(ProportionControl && GetParent() is SpatialControl parent)
        {
            parent.Connect(nameof(SizeChanged), this, nameof(OnReferenceSizeChanged),
                new Godot.Collections.Array { parent });
        }
    }

    public virtual void OnReferenceSizeChanged(SpatialControl reference)
    {
        var s = reference.Size;
        Translation = new Vector3(RelativePosition.x * s.x, RelativePosition.y * s.y, Translation.z);
        Size = RelativeSize * s;
    }
}
