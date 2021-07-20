using Godot;
using System;

public class LootArea : Area, IPickable
{
    TwoFiveDMenu attachedMenu;
    Transform offset;

    public void MouseOn(TwoFiveDMenu _menu)
    {

    }

    public void MouseOff(TwoFiveDMenu _menu)
    {
        
    }

    public void Attach(TwoFiveDMenu menu)
    {
        offset = Transform;
        attachedMenu = menu;
        attachedMenu.Connect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(UpdateGTransform));
        
        offset = attachedMenu.RayEndpoint.GlobalTransform.Inverse() * GlobalTransform;
    }

    public void Detach()
    {
        attachedMenu.Disconnect(nameof(TwoFiveDMenu.MouseUpdated), this, nameof(UpdateGTransform));
    }

    //We're not actually updating our transform to this.
    //but to this offset by what we started with.
    public void UpdateGTransform(Transform t)
    {
        GlobalTransform = t * offset;
    }

}
