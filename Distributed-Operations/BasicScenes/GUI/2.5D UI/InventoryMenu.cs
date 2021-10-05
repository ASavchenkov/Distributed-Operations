using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using ReplicationAbstractions;

public class InventoryMenu : Control
{
    
    Area InventoryWorkspace;
    //Root items that aren't yet "real" and haven't been replicated.
    //Always show up in inventory menu.
    
    public override void _Ready()
    {
        InventoryWorkspace = GetNode<Area>("Viewport/Camera/InventoryWorkspace");
    }
    public void AddRootInvItem(IInvItem item)
    {
        var observer = (DefaultInvPV) EasyInstancer.GenObserver((Node) item,item.ObserverPathInvPV);
        InventoryWorkspace.AddChild(observer);
    }
}
