using Godot;
using System;

//A control node but spatial so you can parent 3d stuff to it.
//And also so you can have good GUI in VR.
public class SpatialControl : Spatial
{
    private Vector2 _Size = new Vector2(1,1);
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
}

