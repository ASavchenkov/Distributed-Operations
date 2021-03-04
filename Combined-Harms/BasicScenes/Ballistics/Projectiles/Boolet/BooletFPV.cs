using Godot;
using System;
using System.Collections.Generic;


public class BooletFPV : ProjectileFPV
{
    public new BooletProvider provider {get => (BooletProvider) _provider; private set => _provider = value;}
}
