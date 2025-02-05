namespace SymphonyScramble.StateMachine.States.InAirStates;

public class EnemyJumpState : EnemyInAirState
{
    public EnemyJumpState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.Enemy.Jump();
    }

    public override void Update()
    {
        base.Update();

        Vector2 velocity = _stateMachine.Enemy.Velocity;
        if (velocity.Y >= 0)
        {
            _stateMachine.TransitionToState(new EnemyFallState(_stateMachine));
            return;
        }
    }
}
