using Godot;
using System;
using System.Collections.Generic;

public class RifleFPVObserver : Spatial, IObserver<RifleProvider>
{
    
    [Export]
    Dictionary<string,NodePath> attachmentMap;

    RifleProvider provider;
    public override void _Ready()
    {
        
    }
    public void Init(RifleProvider provider)
    {
        this.provider = provider;
        this.provider.AttachmentUpdated += OnAttachmentUpdated;
    }

    public void OnAttachmentUpdated(string attachPoint, IProvider attachment)
    {
        Node parentNode = GetNode(attachmentMap[attachPoint]);
        for(int i = 0; i<parentNode.GetChildCount(); i++)
        {
            parentNode.GetChild(i).QueueFree();
        }
        //If null was passed, there is no attachment.
        if(!(attachment is null))
            parentNode.AddChild(attachment.GenerateObserver("FPV"));
    }
}
