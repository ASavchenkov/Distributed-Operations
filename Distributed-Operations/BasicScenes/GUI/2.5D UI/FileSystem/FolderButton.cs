using Godot;
using System;

//I can't believe a button has over 50 lines of code.
//I have failed as a software developer.
public class FolderButton : AnchoredSpatialControl, ITakesInput
{

    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    public bool state = false;
    CSGTorus indicator;

    [Signal]
    public delegate void Clicked(bool newState);

    public override void _Ready()
    {
        base._Ready();

        var area = GetNode<PickableAreaControl>("AreaControl");
        area.PickingMember = new PickingMixin(this, false, nameof(MouseOn), nameof(MouseOff));
        Claims.Claims.UnionWith(M1.Claims.Claims);
        M1.Connect(nameof(MouseActionTracker.FullClick), this, nameof(OnClick));

        indicator = GetNode<CSGTorus>("CSGTorus");
    }

    public void MouseOn(MultiRayCursor cursor)
    {
        M1.cursor = cursor;
    }

    public void MouseOff()
    {
    }

    public void OnClick()
    {
        state = !state;
        if(state)
            indicator.RotationDegrees = new Vector3(90,0,0);
        else
            indicator.RotationDegrees = new Vector3(0,0,0);
        EmitSignal(nameof(Clicked), state);
        //we will do an actual animation in the future.
    }

    public bool OnInput( InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }
}
