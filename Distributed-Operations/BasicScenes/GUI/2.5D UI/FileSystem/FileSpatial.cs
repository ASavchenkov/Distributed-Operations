using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using ReplicationAbstractions;

using MessagePack;
using JsonPrettyPrinterPlus;

public class FileSpatial : SpatialControl, IPickable, IAnchored
{
    public bool Permeable{get;set;} = false;
    public InputClaims Claims {get;set;} = new InputClaims();

    [Export]
    public AnchorMember anchorMember {get;set;}
    
    MouseActionTracker M1 = new MouseActionTracker("MousePrimary");
    
    public Godot.File file = new Godot.File();
    public string Path {get;set;}
    public string DispName;
    SpatialLabel label;

    UserObserver user;

    public override void _Ready()
    {
        Claims = M1.Claims;
        M1.Connect(nameof(MouseActionTracker.Drag), this, nameof(OnDrag));
        
        user  = GetNode<UserObserver>("/root/UserObserver_1");
        label = GetNode<SpatialLabel>("Label");
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
    public void MouseOn(MultiRayCursor cursor)
    {
        M1.cursor = cursor;
    }
    
    public void MouseOff() {}
    
    public bool OnInput(InputEvent inputEvent)
    {
        return M1.OnInput(inputEvent);
    }

    public void OnDrag()
    {
        GD.Print("onDrag: ", Path);
        file.Open(Path, Godot.File.ModeFlags.Read);
        
        string contents = file.GetAsText();
        byte[] bytified = MessagePackSerializer.ConvertFromJson(contents);
        object deserialized = MessagePackSerializer.Typeless.Deserialize(bytified);
        
        if(deserialized is string str)
        {
            GD.Print("it's a string");
            //This is just a file with a scene path. Instance it how we normally would.
            //Assume it's an IInvItem because currently it can't be anything else and work.
            Node instanced = EasyInstancer.Instance<Node>(str);
            GetNode("/root/GameRoot/Assets").AddChild(instanced);
            user.InventoryMenu.AddRootInvItem((IInvItem) instanced);
            GD.Print("instanced: ", instanced.GetPath());
        }
        else if (deserialized is SerializedNode sn)
        {
            GD.Print("it's a serializedNode");
            Node instanced = (Node) sn.Instance(GetTree(), newName: true);
            user.InventoryMenu.AddRootInvItem((IInvItem) instanced);
            GD.Print("deserialized: ", instanced.GetPath());
        }
        file.Close();
    }
}
