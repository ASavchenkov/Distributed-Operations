using Godot;
using System;

public class AnchorMember : Resource
{
    SpatialControl owner;
    SpatialControl parent = null; //parent node of the owner. Null if parent is not SpatialControl
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
    
    public AnchorMember(){}

    //NOT an override of Godot.Object._init
    //That doesn't take arguments and this thing needs a reference to its owner.
    public void Init(SpatialControl o)
    {
        owner = o;
        owner.Connect(nameof(SpatialControl.OnPredelete), this, nameof(OnOwnerDelete));
        // p.GetTree().GetEditedSceneRoot();
        if(owner.GetParent() is SpatialControl p)
        {
            parent = p;
            parent.Connect(nameof(SpatialControl.SizeChanged), this, nameof(OnReferenceSizeChanged),
                new Godot.Collections.Array { parent });
            OnReferenceSizeChanged(parent.Size, parent);
        }
    }

    public virtual void OnReferenceSizeChanged(Vector2 oldSize, SpatialControl reference)
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
            return AnchorRight * reference.Size.x + MarginRight - owner.Translation.x;
        return owner.Size.x;

    }
    float AnchorSY(SpatialControl reference)
    {
        if(Bottom)
            return AnchorBottom * reference.Size.y + MarginRight + owner.Translation.y;
        return owner.Size.y;
    }

    //This is necessary because something else is referencing this object
    //and we dont know what.
    public void OnOwnerDelete()
    {
        if(!(parent is null))
            parent.Disconnect(nameof(SpatialControl.SizeChanged), this, nameof(OnReferenceSizeChanged));
        parent = null;
        owner = null;
    }
}
public interface IAnchored
{
    AnchorMember anchorMember {get;set;}
}