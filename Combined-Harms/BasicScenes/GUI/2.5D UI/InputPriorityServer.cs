using Godot;
using System;
using System.Collections.Generic;


//For those who use Input singleton
//This lets you track whether you have priority
//for a class of inputs you define.
//ALWAYS A NODE.
public interface IPriorityHolder
{
    List<string> Claims {get; set;}
}

//Every subscriber in each layer gets the input that reaches that layer.
//We only check whether input has been handled when we go for the next layer.

public class InputPriorityServer : Node
{

    static InputPriorityServer Instance;
    public static Dictionary<string, ICollection<Node>> layerNameMap {get; private set;}
    static List<ICollection<Node>> priorityMap;

    public const string gameManagement = "gameManagement";
    public const string character = "character";
    public const string menu = "menu";
    public const string mouseOver = "mouseOver";
    public const string selected = "Selected";
    public const string clicked = "Clicked";

    static void InsertLayer(string name, int index)
    {
        var newLayer = new HashSet<Node>();
        priorityMap.Insert(index, newLayer);
        layerNameMap.Add(name, newLayer);
    }

    //If you need to manually control a particular layer.
    //(used by ordered layers most frequently)
    public static void SetLayer(string name, ICollection<Node> newLayer)
    {
        int index = priorityMap.IndexOf(layerNameMap[name]);
        priorityMap[index] = newLayer;
        layerNameMap[name] = newLayer;
    }

    public static void AddLayerBefore(string name, string reference)
    {
        var refIndex = priorityMap.IndexOf(layerNameMap[reference]);
        InsertLayer(name, refIndex);
    }
    public static void AddLayerAfter(string name, string reference)
    {
        var refIndex = priorityMap.IndexOf(layerNameMap[reference]);
        InsertLayer(name, refIndex+1);
    }

    public static void Subscribe(string layerName, Node subscriber)
    {
        //Gonna need to remove and add when things go in and out of tree
        //This is fine since only nodes in tree should take inputs anyways
        subscriber.Connect("tree_exiting", Instance, nameof(Unsubscribe),
            new Godot.Collections.Array(new object[] {subscriber}));
        layerNameMap[layerName].Add(subscriber);
        
    }
    public static void Unsubscribe(Node subscriber)
    {
        subscriber.Disconnect("tree_exiting", Instance, nameof(Unsubscribe));
        foreach(ICollection<Node> layer in priorityMap)
        {
            if(layer.Contains(subscriber))
            {
                layer.Remove(subscriber);
                subscriber.Disconnect("tree_exiting", Instance, nameof(Unsubscribe));
            }
        }
    }


    public static bool CheckPriority(IPriorityHolder holder, string claim)
    {
        foreach( ICollection<Node> layer in priorityMap)
        {
            if(layer.Contains((Node)holder))
                return true;
            foreach(Node n in layer)
            {
                if(n is IPriorityHolder h && h.Claims.Contains(claim))
                    return false;
            }
        }
        GD.PrintErr("Checked entire priorityMap but could not find holder: ", ((Node)holder).Name);
        return true;
    }

    public override void _Ready()
    {
        Instance = this;
        InsertLayer(gameManagement,0);
        InsertLayer(character,0);
        InsertLayer(menu,0);
        InsertLayer(mouseOver,0);
        InsertLayer(selected,0);
        InsertLayer(clicked, 0);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        foreach(ICollection<Node> layer in priorityMap)
        {
            if(GetTree().IsInputHandled()) break;
            foreach(Node n in layer)
            {
                if(layer is List<Node> && GetTree().IsInputHandled())
                    break;
                n._UnhandledInput(inputEvent);
            }
        }
        //To avoid duplicate processing of inputs
        //Unlikely to get here without input being handled
        //but if we do it's good to make sure it's still intercepted.
        GetTree().SetInputAsHandled();
        //Lets be careful about this.
        //Godot may not expect multiple SetInputAsHandled() calls for a single InputEvent.
    }
}
