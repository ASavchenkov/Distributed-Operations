using Godot;
using System;

public class AnchorMember : Godot.Object
{
    SpatialControl owner;

    public bool Left = true;
    public bool Top = true;
    public bool Right = true;
    public bool Bottom = true;
    
    public float AnchorLeft = 0;
    public float AnchorTop = 0;
    public float AnchorRight = 1;
    public float AnchorBottom = 1;
    
    public float MarginLeft = 0;
    public float MarginTop = 0;
    public float MarginRight = 0;
    public float MarginBottom = 0;
    
    public AnchorMember(SpatialControl p)
    {
        owner = p;
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
    AnchorMember aMember {get;set;}
}