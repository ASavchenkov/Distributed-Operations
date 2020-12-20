using Godot;
using System;
using System.Collections.Generic;
public interface IProvider
{
    Node GenerateObserver(string name);
}

public interface IObserver<T> where T: IProvider
{
    void Init(T provider);
}

public interface INOKTransferrable
{
    void OnNOKTransfer(int uid);
}

//Create a static instance in your classes to make creation
public class NodeFactory<T> where T: Node
{
    public string ScenePath {get; private set;}

    public NodeFactory(string scenePath)
    {
        ScenePath = scenePath;
    }

    public T Instance()
    {
        PackedScene scene = GD.Load<PackedScene>(ScenePath);
        return (T) scene.Instance();
    }
    
}
