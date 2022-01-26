using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using ReplicationAbstractions;

public class InventoryMenu : Control
{
    public InputClaims Claims {get;set;} = new InputClaims();

    PickingRay ray;
    PickingRay ray2;
    
    Area InventoryWorkspace;
    //Root items that aren't yet "real" and haven't been replicated.
    //Always show up in inventory menu.

    private OrderedRouter mouseOverRouter = new OrderedRouter();

    public override void _Ready()
    {
        InventoryWorkspace = GetNode<Area>("Viewport/Camera/InventoryWorkspace");
        ray = GetNode<PickingRay>("Viewport/Camera/PickingRay");
        GetNode<UserObserver>("/root/GameRoot/Management/UserObserver_1").Cursor.InsertRay(0, ray);
        
        //Technically we also use mouse motion,
        //but that isn't accessible through the Input singleton
        //So we don't claim it doesn't really exist.
        Claims.Claims.UnionWith(InputPriorityServer.Instance.mouseButtons);
    }
    public void AddRootInvItem(IInvItem item)
    {
        var observer = (DefaultInvPV) EasyInstancer.GenObserver((Node) item,item.ObserverPathInvPV);
        InventoryWorkspace.AddChild(observer);
    }

    public override void _EnterTree()
    {
        //This whole setup is bad and needs to be fixed.
        //perspective PickingRays should be handled by the cameras
        //or whatever is in charge of the cameras.
        //We should only have to ping them to subscribe and unsub.
        var user = GetNode<UserObserver>("/root/GameRoot/Management/UserObserver_1");
        
        if(!(ray is null))
            user.Cursor.InsertRay(0, ray);
        
        if(ray2 is null)
            ray2 = PickingRay.Factory.Instance();
        
        GetViewport().GetCamera().AddChild(ray2);
        user.Cursor.InsertRay(user.Cursor.RayCount, ray2);
        base._EnterTree();
        GD.Print(user.Cursor.RayCount);
        GD.Print(user.Cursor.Enabled);
    }

    public override void _ExitTree()
    {   
        var user = GetNode<UserObserver>("/root/GameRoot/Management/UserObserver_1");
        user.Cursor.RemoveRay(ray);
        user.Cursor.RemoveRay(ray2);
        GetViewport().GetCamera().RemoveChild(ray2);
        base._ExitTree();
        GD.Print(user.Cursor.RayCount);
        GD.Print(user.Cursor.Enabled);
    }

}
