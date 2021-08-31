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