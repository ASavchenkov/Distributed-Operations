using Godot;
using System;

public class RootHandle : Spatial
{
    InventoryMenu attachedMenu;
    Transform offset;

    public void Attach(InventoryMenu menu)
    {
        offset = Transform;
        attachedMenu = menu;
        attachedMenu.Connect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateGTransform));
        
        offset = attachedMenu.RayEndpoint.GlobalTransform.Inverse() * GlobalTransform;
    }

    public void Detach()
    {
        attachedMenu.Disconnect(nameof(InventoryMenu.RayUpdated), this, nameof(UpdateGTransform));
    }

    //We're not actually updating our transform to this.
    //but to this offset by what we started with.
    public void UpdateGTransform(Transform t)
    {
        GlobalTransform = t * offset;
    }

}
