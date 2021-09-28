using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using ReplicationAbstractions;

using MessagePack;
using JsonPrettyPrinterPlus;

public class FileControl : Control, IPickable, FileSystem.IFSControl
{

    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    
    public Godot.File file = new Godot.File();
    public string Path {get;set;}
    public string DispName;
    Label label;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        
        label = GetNode<Label>("Name");
        label.Text = DispName;
    }

    public bool Refresh()
    {
        if(!file.FileExists(Path))
        {
            QueueFree();
            return false;
        }
        return true;
    }
    public void MouseOn(TwoFiveDMenu menu)
    {
        M1.menu = menu;
    }
    
    public void MouseOff() {}
    
    public bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }

    public void OnDrag()
    {
        GD.Print("onDrag");
        file.Open(Path, Godot.File.ModeFlags.Read);
        
        string contents = file.GetAsText();
        byte[] bytified = MessagePackSerializer.ConvertFromJson(contents);
        
        object deserialized = MessagePackSerializer.Typeless.Deserialize(bytified);
        GD.Print(deserialized.GetType());
        
        if(deserialized is string str)
        {
            GD.Print("it's a string");
            //This is just a file with a scene path. Instance it how we normally would.
            //Assume it's an IInvItem because currently it can't be anything else and work.
            Node instancedNode = EasyInstancer.Instance<Node>(str);
            GetNode("/root/GameRoot/Loot").AddChild(instancedNode);
            GD.Print("instanced: ", instancedNode.GetPath());
        }
        else if (deserialized is SerializedNode sn)
        {
            GD.Print("it's a serializedNode");
            Node instanced = (Node) sn.Instance(GetTree(), newName: true);
            GD.Print("deserialized: ", instanced.GetPath());
        }
        file.Close();
    }

    //Not yet used anywhere.
    public void OnDrop( SerializedNode target)
    {
        if(file.Open(Path, Godot.File.ModeFlags.Write) != Error.Ok)
        {
            GD.PrintErr("can't open file bruh");
            return;
        }
        string serialized = MessagePackSerializer.SerializeToJson<SerializedNode>(target);
        GD.Print(serialized);
        file.StoreString(serialized.PrettyPrintJson());
        file.Close();
    }

}
