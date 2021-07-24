using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;


//Every subscriber in each layer gets the input that reaches that layer.
//We only check whether input has been handled when we go for the next layer.

public class InputPriorityServer : Node
{

    public static InputPriorityServer Instance;

    public const string gameManagement = "gameManagement";
    public const string character = "character";
    public const string menu = "menu";
    public const string mouseOver = "mouseOver";
    public const string selected = "Selected";
    public const string dragging = "Dragging";

    public readonly string[] movementActions = {"MoveForward","MoveLeft", "MoveBack", "MoveRight","MoveUp","MoveDown"};
    public readonly string[] mouseButtons = {"MousePrimary", "MouseSecondary"};
    public static NamedLayerRouter BaseRouter = new NamedLayerRouter();


    public override void _Ready()
    {
        Instance = this;
        BaseRouter.layerPriorities = new List<string> {dragging, selected, mouseOver, menu, character, gameManagement};

    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(BaseRouter.OnInput(inputEvent))
            GetTree().SetInputAsHandled();
    }
}


