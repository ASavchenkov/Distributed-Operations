using Godot;
using System;

public class AnimTree : AnimationTree
{
    AnimationNodeStateMachinePlayback stateMachine;
    public override void _Ready()
    {
        stateMachine = (AnimationNodeStateMachinePlayback) Get("parameters/playback");
        stateMachine.Start("Combat Stance");
        // stateMachine.Travel("Combat Stance");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        GD.Print(stateMachine.IsPlaying());
    }
}
