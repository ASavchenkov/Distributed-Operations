using Godot;
using System;
using System.Collections.Generic;

//Doesn't actually change any sizes.
//Just places things in order vertically.
//Not even anchored. Derive it to make an anchored thing.
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

    public void AddSpatialControl(SpatialControl child)
    {
        AddChild(child);
        RegisterChild(child);
    }

    private void RegisterChild(SpatialControl child)
    {
        if(child.GetIndex() ==0)
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