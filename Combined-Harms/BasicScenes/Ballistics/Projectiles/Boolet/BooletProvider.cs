using Godot;
using System;

/*

Boolet represents gamified bullets, specifically the CS:GO/Valorant type.

Boolets have a damage number, and a penetration proportion.
*/

public class BooletProvider : ProjectileProvider
{
    [Export]
    public float Damage;
    [Export]
    public float Penetration;
}


    public class BaseMember { }

    public class BaseOwner
    {
        protected BaseMember _member;
        public BaseMember member {get => _member; private set => _member = value;}
    }

    public class DerivedMember: BaseMember { }

    public class DerivedOwner : BaseOwner
    {
        public new DerivedMember member {get => (DerivedMember) _member; private set => _member = value;}
    }