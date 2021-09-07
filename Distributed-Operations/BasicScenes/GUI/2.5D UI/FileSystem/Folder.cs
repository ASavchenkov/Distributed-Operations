using Godot;
using System;

public class Folder : DraggableArea
{

    public override void _Ready()
    {
        Connect("tree_entered", this, nameof(OnTreeEntered));
        
    }

    public void OnTreeEntered()
    {

    }

    public override void OnMouseUpdate()
    {
        //does nothing. I should really rethink this being an abstract.
    }
}
