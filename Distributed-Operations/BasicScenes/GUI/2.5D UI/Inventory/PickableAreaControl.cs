using Godot;
using System;

public class PickableAreaControl : AreaControl, IPickable
{
    public PickingMixin PickingMember {get;set;}
}
