using Godot;
using System;

//A control node but spatial so you can parent 3d stuff to it.
//And also so you can have good GUI in VR.
public class SpatialControl : Spatial
{
    private float changeThreshold = 1e-6f;
    private Vector2 _Size = new Vector2(1,1);
    [Export]
    public Vector2 Size
    {
        get => _Size;
        set
        {
            var oldSize = _Size;
            _Size = value;
            EmitSignal(nameof(SizeChanged), oldSize);
        }
    }

    [Signal]
    public delegate void SizeChanged(Vector2 oldSize);
}

