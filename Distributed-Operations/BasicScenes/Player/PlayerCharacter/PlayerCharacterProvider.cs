using Godot;
using System;
using System.Collections.Generic;

using MessagePack;
using JsonPrettyPrinterPlus;

using ReplicationAbstractions;

public class PlayerCharacterProvider : Node, IHasFPV, IHas3PV, IInvItem
{

    public ReplicationMember rMember {get; set;} = new ReplicationMember();

    public static NodeFactory<PlayerCharacterProvider> Factory = 
        new NodeFactory<PlayerCharacterProvider> ("res://BasicScenes/Player/PlayerCharacter/PlayerCharacterProvider.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}

    public InvSlot parent {get;set;} = null;

    [Export]
    public string ObserverPathFPV { get; set;}
    [Export]
    public string ObserverPath3PV { get; set;}
    [Export]
    public string ObserverPathInvPV {get; set;}
    //For generating observers

    [Signal]
    public delegate void TrajectoryUpdated(Vector3 translation, Vector3 yaw, Vector3 pitch, Vector3 velocity);

    Vector3 Translation = new Vector3();
    Vector3 YawRotation = new Vector3();
    Vector3 PitchRotation = new Vector3();

    [Export]
    public float maxSpeed {get; private set;} = 10;
    [Export]
    public float acceleration {get; private set;} = 10;
    [Export]
    public float jumpImpulse {get; private set;} = 10;
    [Export]
    public float maxPitch {get; private set;} = 80; //degrees

    [Export]
    public float HP = 100;
    [Export]
    public float Armor = 50;

    public InvSlot ChestSlot;
    public InvSlot HandSlot;

    //Needed to put stuff back where it belongs later.
    //specific to PlayerCharacterProvider BC humanoids lol.
    public InvSlot HandItemHome;

    public object GetData()
    {
        return new SaveData(this);
    }

    [MessagePackObject]
    public class SaveData
    {
        [Key(0)]
        public SerialVector3 Translation;
        [Key(1)]
        public SerialVector3 YawRotation;
        [Key(2)]
        public SerialVector3 PitchRotation;

        [Key(3)]
        public InvSlot.SaveData ChestSlot;
        [Key(4)]
        public InvSlot.SaveData HandSlot;
        
        public SaveData(){}
        public SaveData(PlayerCharacterProvider target)
        {
            Translation = new SerialVector3(target.Translation);
            YawRotation = new SerialVector3(target.YawRotation);
            PitchRotation = new SerialVector3(target.PitchRotation);

            ChestSlot = target.ChestSlot.GetData();
            HandSlot = target.HandSlot.GetData();
        }
    }

    public void ApplyData(object data)
    {
        SaveData casted = (SaveData) data;
        Translation = casted.Translation.Deserialize();
        YawRotation = casted.YawRotation.Deserialize();
        PitchRotation = casted.PitchRotation.Deserialize();
        
    }

    public override void _Ready()
    {
        this.ReplicableReady();
        var MapNode = GetNode("/root/GameRoot/GameWorld/Map");
        
        ChestSlot = GetNode<InvSlot>("ChestSlot");
        HandSlot = GetNode<InvSlot>("HandSlot");

        var rifle = EasyInstancer.Instance<RifleProvider>("res://BasicScenes/Items/Gun/Rifle/M4A1/M4A1Provider.tscn");
        GetNode("/root/GameRoot/GameWorld/Assets").AddChild(rifle);
        SetHandItem((IInvItem) rifle, ChestSlot);
        
        // GD.Print(new SerializedNode(this).AsJson().PrettyPrintJson());
        
    }

    public void OnNOKTransfer(int uid)
    {
        QueueFree();
    }

    public void SetHandItem(IInvItem item, InvSlot home)
    {
        //I wish this was simpler but it isn't.
        //Just swapping items around.
        //This might actually have to happen slower for gameplay reasons.
        //(so it'l be even more complicated.)

        
        if(HandItemHome != null)
            HandItemHome.Occupant = HandSlot.Occupant;
        HandSlot.Occupant = item;
        if(home != null)
            HandItemHome = home;
        HandItemHome.Occupant = null;
    }

    //stateUpdate will be used once it has weight information
    public bool Validate(IInvItem occupant, object stateUpdate)
    {
        if(occupant == this)
            return false;
        return true;
    }

    [PuppetSync]
    public void UpdateTrajectory(Vector3 translation, Vector3 yaw, Vector3 pitch, Vector3 velocity)
    {
        Translation = translation;
        YawRotation = yaw;
        PitchRotation = pitch;

        EmitSignal(nameof(TrajectoryUpdated), translation, yaw, pitch, velocity);
    }

    [Master]
    public void HitRPC( float damage, float pen, string part)
    {
        GD.Print("Got hit from: ", part);
        var armorDamage = damage * (1-pen);
        Armor -= armorDamage;
        if(Armor < 0)
        {
            HP += Armor;
            Armor = 0;
        }
        HP -= (damage * pen);

        Rpc(nameof(UpdateHP), HP, Armor);
    }

    [Puppet]
    public void UpdateHP( float hp, float armor)
    {
        HP = hp;
        Armor = armor;
    }
}
