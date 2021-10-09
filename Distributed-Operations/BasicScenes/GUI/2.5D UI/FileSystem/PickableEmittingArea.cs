using Godot;
using System;

public class PickableEmittingArea : ControlledBoxArea, IPickable
{
    [Export]
    NodePath OwnerPath = "..";
    IPickable owner;

    public override void _Ready()
    {
        base._Ready();
        owner = (IPickable)GetNode(OwnerPath);
    }

    public bool Permeable {get => owner.Permeable; set{ owner.Permeable =value;}}
    public void MouseOn(MultiRayCursor cursor) => owner.MouseOn(cursor);
    public void MouseOff() => owner.MouseOff();

    public InputClaims Claims{get =>owner.Claims; set {owner.Claims = value;}}
    public bool OnInput(InputEvent inputEvent) => owner.OnInput(inputEvent);
}
