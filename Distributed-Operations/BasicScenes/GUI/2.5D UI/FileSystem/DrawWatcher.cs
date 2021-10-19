using Godot;
using System;

public class DrawWatcher : HBoxContainer
{
    public override void _Ready()
    {
        Connect("draw", this, nameof(OnDraw)); 
    }

    public void OnDraw()
    {
        GD.PrintErr(((Folder) GetParent()).DispName);
    }
}
