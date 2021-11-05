using Godot;
using System;

public class FolderSpatial : SpatialVBoxContainer, IPickable
{

    public bool Permeable {get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    protected MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    protected MultiRayCursor cursor = null;
    
    SpatialLabel label;

    public override void _Ready()
    {
        label = GetNode<SpatialLabel>("Label");
        Claims = M1.Claims;// Just link to M1 for now since it's the only one.
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
    }

    public virtual void MouseOn(MultiRayCursor _cursor)
    {
        GD.Print(Name, ": Moused on");
        cursor = _cursor;
        M1.cursor = cursor;
    }

    //Allow the moused on thing to request that focus is kept.
    public virtual void MouseOff()
    {
        GD.Print(Name, ": Moused off");   
    }

    public virtual void OnDrag()
    {
        GD.Print("OnDrag");
    }

    public virtual bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }

}

