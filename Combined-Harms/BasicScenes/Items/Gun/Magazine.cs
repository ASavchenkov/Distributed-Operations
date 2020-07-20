using Godot;
using System;

public class Magazine : Node, IMunitionSource
{
    
    string ProjectileScene = "res://BasicScenes/Projectiles/Boolet/Boolet.tscn";
    int currentAmount = 30;

    public string DequeueMunition()
    {
        if(currentAmount == 0)
        {
            return null;
        }else
        {
            currentAmount--;    
            return ProjectileScene;
        }
    }
}
