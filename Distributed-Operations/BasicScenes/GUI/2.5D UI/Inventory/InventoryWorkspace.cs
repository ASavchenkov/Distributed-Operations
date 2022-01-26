using Godot;
using System;

public class InventoryWorkspace : Area, IPickable
{
    public PickingMixin PickingMember {get;set;} = new PickingMixin();
}
