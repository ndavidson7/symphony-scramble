﻿namespace SymphonyScramble;

public class BounceState : InAirState
{
    public BounceState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        /*Vector2 velocity = _stateMachine.Player.Velocity;
        velocity.Y = (-_stateMachine.Player.Velocity.Y-1)*50;
        _stateMachine.Player.Velocity = velocity;*/
        _stateMachine.Player.Bounce();
    }

    public override void Update()
    {
        base.Update();
        Vector2 velocity = _stateMachine.Player.Velocity;
        if (velocity.Y >= 0)
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Fall);
            return;
        }
    }
}
