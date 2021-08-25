using Godot;
using System;
using System.Collections.Generic;

using MessagePack;

using ReplicationAbstractions;


[MessagePackObject]
public class SerializedNode
{
    [Key(0)]
    public string Parent;
    [Key(1)]
    public string Name;
    [Key(2)]
    public string ScenePath;

    public SerializedNode(IReplicable target)
    {
        Parent = target.GetParent().GetPath();
        Name = target.Name;
        ScenePath = target.ScenePath;
    }
    //Override and call base Instance to do actual deserialization.    
    public virtual IReplicable Instance(SceneTree tree)
    {
        Node instance = EasyInstancer.Instance<Node>(ScenePath);
        instance.Name = Name;
        tree.Root.GetNode(Parent).AddChild(instance);
        //This is awful, but all IReplicables are Nodes.
        //(There's just no way to stipulate that in c#)
        return (IReplicable) instance;
    }
}