using Godot;
using System;
using System.Collections.Generic;


//For those who use Input singleton
//This lets you track whether you have priority
//for a class of inputs you define.
//ALWAYS A NODE.
public interface PriorityHolder
{
    List<string> Claims {get; set;}
}

//Every subscriber in each layer gets the input that reaches that layer.
//We only check whether input has been handled when we go for the next layer.

public class InputPriorityServer : Node
{

    static InputPriorityServer Instance;
    static Dictionary<string, HashSet<Node>> layerNameMap;
    static List<HashSet<Node>> priorityMap;

    static void InsertLayer(string name, int index)
    {
        var newLayer = new HashSet<Node>();
        priorityMap.Insert(index, newLayer);
        layerNameMap.Add(name, newLayer);
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
        foreach(HashSet<Node> layer in priorityMap)
        {
            if(layer.Contains(subscriber))
            {
                layer.Remove(subscriber);
                subscriber.Disconnect("tree_exiting", Instance, nameof(Unsubscribe));
            }
        }
    }


    public static bool CheckPriority(PriorityHolder holder, string claim)
    {
        foreach( HashSet<Node> layer in priorityMap)
        {
            if(layer.Contains((Node)holder))
                return true;
            foreach(Node n in layer)
            {
                if(n is PriorityHolder h && h.Claims.Contains(claim))
                    return false;
            }
        }
        GD.PrintErr("Checked entire priorityMap but could not find holder: ", ((Node)holder).Name);
        return true;
    }

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        foreach(HashSet<Node> layer in priorityMap)
        {
            if(GetTree().IsInputHandled()) break;
            foreach(Node n in layer)
            {
                n._UnhandledInput(inputEvent);
            }
        }
        //To avoid duplicate processing of inputs
        //Unlikely to get here without input being handled
        //but if we do it's good to make sure it's still intercepted.
        GetTree().SetInputAsHandled();
        //Lets be careful about this.
        //Godot may not expect multiple SetInputAsHandled() calls.
    }
}
