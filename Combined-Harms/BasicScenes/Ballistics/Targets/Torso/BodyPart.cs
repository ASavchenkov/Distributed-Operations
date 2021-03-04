using Godot;
using System;
using System.Collections.Generic;
public class BodyPart : Node, IBallisticTarget
{

    [Signal]
    public delegate void Hit(int damage, int pen);

    [Export]
    public float DamageModifier;
    
    public void OnContact(ProjectileFPV p)
    {
        p = (BooletFPV) p;
        EmitSignal(nameof(Hit), p.provider.Damage * DamageModifier, p.provider.pen);
    }
}
