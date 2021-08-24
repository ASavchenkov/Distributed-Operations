using Godot;
using System;
using System.Collections.Generic;

using MessagePack;

using ReplicationAbstractions;


//Bad name. Will improve later.
//Since a lot of things need to be saved in groups of interconnected objects,
//We save them all at the same time.
public class Serializer : Node
{
    public static Serializer Instance;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
    //Instances all of the ISaveables in a set.
    //(It's a set because it's easier to serialize that way)
    //(but maybe we can build it as a set then serialize/deserializer as an array);
    public Dictionary<Guid, ISaveable> InstanceContext(HashSet<ISavedObject> SavedObjects)
    {
        //Only used during deserialization by references to other saved objects.
        //Not serialized.
        Dictionary<Guid, ISaveable> context = new Dictionary<Guid, ISaveable>();

        foreach(ISavedObject o in SavedObjects)
        {
            context.Add(o.ID, o.Instance());
        }
        
        foreach(ISavedObject o in SavedObjects)
            context[o.ID].ApplySaveData(o.SaveData, context);
        
        return context;
    }
    
}

public interface ISaveable
{
    void ApplySaveData(byte[] data, Dictionary<Guid, ISaveable> context);
}

public interface ISavedObject
{
    Guid ID {get;set;}
    //Object specific data. The instanced object
    //can deserialize this later.
    byte[] SaveData{get;set;}
    ISaveable Instance();
}

public class SavedNode : ISavedObject
{
    
    public byte[] SaveData {get;set;}
    public Guid ID {get;set;}

    //Data needed to restore every node.
    public string ScenePath;
    public string ParentPath;

    public ISaveable Instance()
    {
        var instanced = EasyInstancer.Instance<Node>(ScenePath);
        Serializer.Instance.GetNode(ParentPath).AddChild(instanced);
        return (ISaveable) instanced;
    }
}

