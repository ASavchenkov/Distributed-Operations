using Godot;
using System;

public class AnchorMember : Resource
{
    SpatialControl owner;

    [Export]
    public bool Left = true;
    [Export]
    public bool Top = true;
    [Export]
    public bool Right = true;
    [Export]
    public bool Bottom = true;
    
    [Export]
    public float AnchorLeft = 0;
    [Export]
    public float AnchorTop = 0;
    [Export]
    public float AnchorRight = 1;
    [Export]
    public float AnchorBottom = 1;
    
    [Export]
    public float MarginLeft = 0;
    [Export]
    public float MarginTop = 0;
    [Export]
    public float MarginRight = 0;
    [Export]
    public float MarginBottom = 0;
    
    public AnchorMember()
    {

    }

    public void Init(SpatialControl p)
    {
        owner = p;
        // p.GetTree().GetEditedSceneRoot();
        if(owner.GetParent() is SpatialControl parent)
        {
            parent.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnReferenceSizeChanged),
                new Godot.Collections.Array { parent });
        }
    }

    public virtual void OnReferenceSizeChanged(SpatialControl reference)
    {
        owner.Translation = new Vector3(AnchorX(reference), AnchorY(reference), owner.Translation.z);
        owner.Size = new Vector2(AnchorSX(reference), AnchorSY(reference));
    }

    float AnchorX(SpatialControl reference)
    {
        if(Left)
            return AnchorLeft * reference.Size.x + MarginLeft;
        return owner.Translation.x;
    }
    float AnchorY(SpatialControl reference)
    {
        if(Top)
            return AnchorTop * reference.Size.y + MarginTop;
        return owner.Translation.y;
    }
    float AnchorSX(SpatialControl reference)
    {
        if(Right)
            return AnchorRight * reference.Size.x + MarginRight;
        return owner.Size.x;

    }
    float AnchorSY(SpatialControl reference)
    {
        if(Bottom)
            return AnchorBottom * reference.Size.y + MarginRight;
        return owner.Size.y;
    }
}
public interface IAnchored
{
    AnchorMember anchorMember {get;set;}
}