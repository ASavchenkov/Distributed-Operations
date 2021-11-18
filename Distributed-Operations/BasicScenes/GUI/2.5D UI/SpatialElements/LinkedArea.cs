using Godot;
using System;

//for when you want the parent to not be the node parent.
public class LinkedArea : AreaControl
{
    [Export]
    public NodePath ParentPath;
}
