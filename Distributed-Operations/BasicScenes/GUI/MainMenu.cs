using Godot;
using System;

//Function is exclusively to intercept all input events other than Escape
public class MainMenu : CenterContainer, ITakesInput
{

    public InputClaims Claims {get;set;} = new InputClaims();

    public override void _Ready()
    {
        InputPriorityServer.Base.Subscribe(this, BaseRouter.mainMenu);
        Claims.Claims.Add("ui_cancel");
    }
    
    public bool OnInput(InputEvent inputEvent)
    {
        if(IsInsideTree())
        {
            //The buck stops here.
            //Doesn't matter if we need the event.
            if(inputEvent.IsActionPressed("ui_cancel"))
            {
                GetParent().RemoveChild(this);
                InputPriorityServer.RefreshMouseMode();
            }
            return true;
        }
        return false;        
    }

    public override void _EnterTree()
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        base._EnterTree();
    }
}
