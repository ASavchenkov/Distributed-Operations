using Godot;
using System;

//for when you want the parent to not be the node parent.
public class LinkedArea : Area
{
    [Export]
    public NodePath ParentPath;
}
