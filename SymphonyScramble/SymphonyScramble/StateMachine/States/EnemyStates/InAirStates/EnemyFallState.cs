using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble.StateMachine.States.InAirStates;

public class EnemyFallState : EnemyInAirState
{
    public EnemyFallState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Vector2 velocity = _stateMachine.Enemy.Velocity;
        velocity.Y = 0;
        _stateMachine.Enemy.Velocity = velocity;
    }

    public override void Exit()
    {
        base.Exit();

        Vector2 velocity = _stateMachine.Enemy.Velocity;
        velocity.Y = 0;
        _stateMachine.Enemy.Velocity = velocity;
    }

    public override void Update()
    {
        base.Update();


        // Check if landed on ground
        if (_stateMachine.Enemy.IsOnGround)
        {
            EnemyLandState LandState = new EnemyLandState(_stateMachine);
            _stateMachine.TransitionToState(LandState);
            return;
        }
    }
}
