using Godot;
using System;
using System.Collections.Generic;

using MessagePack;

//A Node that alerts others when its occupant or positon is changed.
//Used on providers to store what loot is in their slots.

//NOTES:
//  parent of InvSlot NEEDS to be an IInvItem (or null)
//  Occupant of InvSlot NEEDS to be an IInvItem (or null)
//This allows us to check for potential circular dependencies
//When moving things around.

//WRT: ISaveable, this node isn't dynamically created,
//so there's no need to put it into the context.
//(It's always a child of whatever is being serialized,
//and is accessed through that parent.)
public class InvSlot : Node
{

    private IInvItem _Occupant = null;
    public IInvItem Occupant 
    {
        get { return _Occupant;}
        //Setting "Occupant" triggers an RPC
        //(if you're the master)
        set 
        {
            if(_Occupant == value) return;
            _Occupant = value;
            if(_Occupant is null && IsNetworkMaster())
                Rpc(nameof(NullOccupantRPC));
            else
            {
                GD.Print("Occupant being set to: ", value.Name);
                _Occupant.parent = this;
                if(IsNetworkMaster())
                    Rpc(nameof(OccupantRPC), (_Occupant).GetPath());
            }
            
            //For those who want to keep track of changes
            //(such as parents and observers)
            EmitSignal(nameof(OccupantSet), _Occupant);
        }
    }

    [MessagePackObject]
    public class SaveData
    {
        public SerializedNode Occupant;
        public SaveData( InvSlot target)
        {
            Occupant = target.Occupant.Serialize();
        }
    }
    public void ApplySaveData(SaveData sd)
    {
        Occupant = (IInvItem) sd.Occupant.Instance(GetTree());
    }
    
    [Puppet]
    public void OccupantRPC(NodePath occupant)
    {
        Occupant = (IInvItem) GetNode(occupant);
    }

    [Puppet]
    public void NullOccupantRPC()
    {
        Occupant = null;
    }

    private Vector3? _Translation = null;
    public Vector3? Translation 
    {
        get { return _Translation;}
        set 
        {
            _Translation = value;
            EmitSignal(nameof(TranslationSet), value);
            
        }
    }
    [Signal]
    public delegate void OccupantSet(Node n);
    [Signal]
    public delegate void TranslationSet(Vector3 t);    


    public IInvItem parent = null;

    public override void _Ready()
    {
        parent = (IInvItem) GetParent();
    }

    //InvSlot validation code only checks for compatibility
    //passes to parent; recursive until tree root.
    //occupant may be the same as Occupant, or it may be a new one.
    //stateUpate is an object, passed to parent.
    public virtual bool Validate(IInvItem occupant, object stateUpdate)
    {
        if(occupant is null)
            return true; //"removing" an item is always valid.
        else if(!(parent is null))
            //check with parent for non-obvious failure conditions (if parent exists)
            return parent.Validate(occupant, stateUpdate);
        else return true;
            //we're the root (which in hindsight doesn't ever happen for InvSlots for now)
            //but who knows maybe when the codebase changes...
    }

    //Not quite the same as just setting the property "Occupant"
    //Doesn't auto swap.
    public bool AcceptItem(IInvItem newOccupant)
    {
        
        //Check that we're open
        //Check that newOccupant is compatible with this slot.
        if(Occupant is null && Validate(newOccupant, null))
        {
            //if so, remove occupant from old spot, and add it to new spot.
            if(!(newOccupant.parent is null))
                newOccupant.parent.Occupant = null;
            else
                ((Node)newOccupant).QueueFree();
            Occupant = newOccupant;
            return true;
        }
        return false;
    }
}