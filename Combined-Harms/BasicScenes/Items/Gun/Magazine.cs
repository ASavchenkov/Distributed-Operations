using Godot;
using System;
using System.Collections.Generic;
using ReplicationAbstractions;

public class Magazine : Node, IMunitionSource
{

    //IReplicable boilerplate

    public static NodeFactory<Magazine> Factory
        = new NodeFactory<Magazine>("res://BasicScenes/Items/Gun/Magazine.tscn");
    
    public string ScenePath {get => Factory.ScenePath;}
    public HashSet<int> Unconfirmed {get;set;}


    string ProjectileScene = "res://BasicScenes/Projectiles/ProjectileProvider.tscn";
    int currentAmount = 30;

    public string DequeueMunition()
    {
        GD.Print(currentAmount);
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
