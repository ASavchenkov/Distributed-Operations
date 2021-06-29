using Godot;
using System;
using System.Collections.Generic;

public class InputPriorityServer : Node
{

    Dictionary<string, HashSet<Node>> layerNameMap;
    List<HashSet<Node>> priorityMap;

    private void InsertLayer(string name, int index)
    {
        var newLayer = new HashSet<Node>();
        priorityMap.Insert(index, newLayer);
        layerNameMap.Add(name, newLayer);
    }

    public void AddLayerBefore(string name, string reference)
    {
        var refIndex = priorityMap.IndexOf(layerNameMap[reference]);
        InsertLayer(name, refIndex);
    }
    public void AddLayerAfter(string name, string reference)
    {
        var refIndex = priorityMap.IndexOf(layerNameMap[reference]);
        InsertLayer(name, refIndex+1);
    }

    public void Subscribe(string layerName, Node subscriber)
    {
        layerNameMap[layerName].Add(subscriber);
    }
    public void Unsubscribe(string layerName, Node subscriber)
    {
        layerNameMap[layerName].Remove(subscriber);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        foreach(HashSet<Node> layer in priorityMap)
        {
            foreach(Node n in layer)
            {
                n._UnhandledInput(inputEvent);
            }
        }
        GetTree().SetInputAsHandled();
    }
}
