using Godot;
using System;
using System.Collections.Generic;

public class SpatialVBoxContainer : SpatialControl
{


    private void SetBelow(SpatialControl reference, SpatialControl actual)
    {
        var y = reference.Translation.y + reference.Size.y;
        actual.Translation = new Vector3(0, y , actual.Translation.z);
    }
    
    public override void _Ready()
    {
        foreach(Node n in GetChildren())
        {
            if(n is SpatialControl sctrl)
                RegisterChild((SpatialControl) n);
        }
        base._Ready();
    }

    public void AddChild(Node child, int index = -1)
    {
        AddChild(child);
        if(child is SpatialControl sctrl)
            RegisterChild(sctrl);
    }

    private void RegisterChild(SpatialControl child)
    {
        if(child.GetIndex() !=0)
            child.Translation = Vector3.Back * child.Translation.z;
        else
        {
            var previousSC = (SpatialControl) GetChild(child.GetIndex() -1 );
            SetBelow(previousSC, child);
            child.Connect(nameof(SizeChanged), this, nameof(OnChildSizeChanged),
                new Godot.Collections.Array {child});
        }
    }

    public void OnChildSizeChanged(SpatialControl child)
    {
        for (int i = child.GetIndex() + 1; i< GetChildCount(); i++)
        {
            var nextChild = (SpatialControl) GetChild(i);
            SetBelow(child, nextChild);
        }
        child.GetIndex();
    }
}



//Add instance to class if you want it to scale based off of your classes parent.
public class ProportionControlMember : Godot.Object
{
    SpatialControl parent;
    public Vector2 RelativePosition; //(0,0) is top left, (1,-1) is bottom right.
    public Vector2 RelativeSize;
    public ProportionControlMember( SpatialControl _parent)
    {
        ((Spatial) parent).GetViewport().Connect("size_changed", this, nameof(OnReferenceSizeChanged));
    }

    public void OnReferenceSizeChanged(Vector2 size)
    {
        parent.Translation = new Vector3(RelativePosition.x * size.x, RelativePosition.y * size.y, parent.Translation.z);
        parent.Size = RelativeSize * size;
    }

}