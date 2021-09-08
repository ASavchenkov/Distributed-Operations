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

    public readonly string[] movementActions = {"MoveForward","MoveLeft", "MoveBack", "MoveRight","MoveUp","MoveDown"};
    public readonly string[] mouseButtons = {"MousePrimary", "MouseSecondary"};
    public static BaseRouter Base = new BaseRouter();


    public override void _Ready()
    {
        Instance = this;

    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(Base.OnInput(inputEvent))
            GetTree().SetInputAsHandled();
    }
}
public class BaseRouter: NamedLayerRouter
{
    public const string gameManagement = "gameManagement";
    public const string character = "character";
    public const string menu = "menu";
    public const string mouseOver = "mouseOver";
    public const string selected = "Selected";
    public const string dragging = "Dragging";
    public BaseRouter()
    {
        layerPriorities = new List<string> {dragging, selected, mouseOver, menu, character, gameManagement};
    }
}

//When using the Input Singleton, InputClaims lets you track whether
//someone else with higher priority is using the same action.
public class InputClaims : Godot.Object
{
    //Is a reference to another InputClaims PostClaims.
    //Kinda like a linked list pointer... ish.
    public HashSet<string> PriorClaims = new HashSet<string>();
    //These are your claims.
    //Use to check if you need to propagate claims back to parent.
    public HashSet<string> Claims = new HashSet<string>();
    //Union of PriorClaims and Claims.
    public HashSet<string> PostClaims = new HashSet<string>();

    [Signal]
    public delegate void PostClaimUpdate(InputClaims claimsObject);

    public void RecomputePosts(InputClaims priorClaimsObject)
    {
        
        PriorClaims = priorClaimsObject.PostClaims; //Most likely self assignment
        //but necessary if the source changed entirely.
        HashSet<string> newPosts = new HashSet<string>();
        newPosts.UnionWith(PriorClaims);
        newPosts.UnionWith(Claims);
        if(!newPosts.SetEquals(PostClaims))
        {
            PostClaims = newPosts;
            EmitSignal(nameof(PostClaimUpdate), this);
        }
    }

    //if someone prior has claimed this action, return false regardless.
    //Pretty much only needed for IsActionPressed.
    public bool IsActionPressed(string action)
    {
        if(!PriorClaims.Contains(action) && Input.IsActionPressed(action))
            return true;
        return false;
    }
}

public interface ITakesInput
{
    InputClaims Claims {get;set;}
    //Return whether or not you consumed the event.
    bool OnInput(InputEvent inputEvent);
}

//Has a list of strings.
//Nodes and other routers "subscribe" to layers.
//One subscriber per layer.
public class NamedLayerRouter : Godot.Object, ITakesInput
{
    
    Dictionary<string, ITakesInput> layerMap = new Dictionary<string, ITakesInput>();
    public List<string> layerPriorities = new List<string>();

    public InputClaims Claims {get;set;} = new InputClaims();

    public bool OnInput(InputEvent inputEvent)
    {
        foreach(string s in layerPriorities)
        {
            if(layerMap.ContainsKey(s) && layerMap[s].OnInput(inputEvent))
                return true;
        }
        return false;
    }

    public void OnChildPostUpdate(InputClaims claims, string layer)
    {
        foreach(string l in layerPriorities.SkipWhile( ll => ll ==layer))
        {
            if(layerMap.ContainsKey(l))
            {
                layerMap[l].Claims.RecomputePosts(claims);
                return;
            }
        }
        //if this has been called on us by the final child, we emit our own update.
        Claims.RecomputePosts(claims);
    }

    public void Subscribe( ITakesInput newChild, string layer)
    {
        if(!layerPriorities.Contains(layer))
            GD.PrintErr("layer <", layer, "> does not exist.");
        else if(layerMap.ContainsKey(layer))
            GD.PrintErr("layer <", layer, "> is occupied.");
        else
        {
            layerMap[layer] = newChild;
            newChild.Claims.Connect(nameof(InputClaims.PostClaimUpdate), this, nameof(OnChildPostUpdate),
                new Godot.Collections.Array {layer});
        }
    }
    public void Unsubscribe(ITakesInput thing, string layer)
    {
        if(layerMap.ContainsKey(layer) && layerMap[layer] == thing)
        {
            layerMap.Remove(layer);
            thing.Claims.Disconnect(nameof(InputClaims.PostClaimUpdate), this, nameof(OnChildPostUpdate));
        }
        else
        {
            GD.PrintErr("Tried to unsubscribe someone else's layer");
            GD.PrintErr(layer);
        }
    }
}

//DOES NOT SUPPORT CLAIM SYSTEM INTERNALLY
//WILL SIMPLY PASS INCOMING CLAIMS ON
//This is due to the fact that the order can change quite often.
//and updating claims that often may not be a good idea
//(and takes some doing too. So maybe in the future?)
public class OrderedRouter : Godot.Object, ITakesInput
{
    public InputClaims Claims {get;set;} = new InputClaims();

    public List<ITakesInput> LayerPriorities = new List<ITakesInput>();

    public bool OnInput(InputEvent inputEvent)
    {

        foreach(ITakesInput i in LayerPriorities)
        {
            if(i.OnInput(inputEvent))
                return true;
        }
        return false;
    }
}