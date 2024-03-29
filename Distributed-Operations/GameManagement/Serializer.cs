using Godot;
using System;
using System.Collections.Generic;

using MessagePack;
using MessagePack.Internal;
using MessagePack.Formatters;

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
    [Key(4)]
    public object Data;

    public SerializedNode(){}

    public SerializedNode(IReplicable target)
    {
        Parent = target.GetParent().GetPath();
        Name = target.Name;
        ScenePath = target.ScenePath;
        Data = ((ISaveable) target).GetData();
    }
    //Override and call base Instance to do actual deserialization.    
    public virtual IReplicable Instance(SceneTree tree, bool newName = false)
    {
        IReplicable instance = (IReplicable) EasyInstancer.Instance<Node>(ScenePath);
        if(newName)
            instance.rMember.GenName();
        else
            instance.Name = Name;
        tree.Root.GetNode(Parent).AddChild((Node) instance);
        ((ISaveable)instance).ApplyData(Data);
        return instance;
    }

    public byte[] AsBytes()
    {
        return MessagePackSerializer.Serialize<SerializedNode>(this);
    }
    public string AsJson()
    {
        return MessagePackSerializer.SerializeToJson<SerializedNode>(this);
    }
}

[MessagePackObject]
public class SerialVector3
{
    [Key(0)]
    public float x;
    [Key(1)]
    public float y;
    [Key(2)]
    public float z;

    public SerialVector3(){}
    public SerialVector3(Vector3 target)
    {
        x = target.x;
        y = target.y;
        z = target.z;
    }

    public Vector3 Deserialize()
    {
        return new Vector3(x,y,z);
    }

}