using Godot;
using System;
using System.Collections.Generic;
public class BallisticTorso : BallisticTarget
{

    public override void _Ready()
    {
        
    }

    //RPC for projectiles to call.
    [Master]
    public void Hit()
    {
        GD.Print("GOT HIT CALL");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
