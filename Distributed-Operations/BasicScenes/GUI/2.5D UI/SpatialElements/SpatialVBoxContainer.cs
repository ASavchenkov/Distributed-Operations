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
        //Size is positive, but we want to go down so we subtract.
        var y = reference.Translation.y - reference.Size.y;
        actual.Translation = new Vector3(actual.Translation.x, y , actual.Translation.z);
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

    public void InsertSpatialControl(SpatialControl child, int index)
    {
        AddChildBelowNode(GetChild(index), child);
        RegisterChild(child);
    }

    private void RegisterChild(SpatialControl child)
    {
        GD.Print(child.Name, ": ", child.GetIndex());
        if(child.GetIndex() ==0)
            child.Translation = new Vector3(child.Translation.x, 0, child.Translation.z);
        else
        {
            var previousSC = (SpatialControl) GetChild(child.GetIndex() -1 );
            SetBelow(previousSC, child);
            child.Connect(nameof(SizeChanged), this, nameof(OnChildSizeChanged),
                new Godot.Collections.Array {child});
        }
        OnChildSizeChanged(Vector2.Zero, child);
    }

    public void OnChildSizeChanged(Vector2 oldSize, SpatialControl child)
    {
        if(Math.Abs(oldSize.y - child.Size.y) < 1e-7)
            return; //we don't actually care about changes that don't affect vertical size.
            
        GD.Print(child.Name, "; ", child.Translation, "; ", oldSize, " -> ", child.Size);
        for (int i = child.GetIndex() + 1; i< GetChildCount(); i++)
        {
            var nextChild = (SpatialControl) GetChild(i);
            SetBelow(child, nextChild);
            child = nextChild;
        }
    }
}