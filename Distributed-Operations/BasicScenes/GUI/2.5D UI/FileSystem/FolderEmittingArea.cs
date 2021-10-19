using Godot;
using System;

//Folder accepts dropping items, so we need to additionally pass that on.
public class FolderEmittingArea : PickableEmittingArea, IAcceptsItem
{
    public bool AcceptItem( DefaultInvPV item)
    {
        return ((IAcceptsItem) owner).AcceptItem(item);
    }
}
