using SymphonyScramble.StateMachine.States.InAirStates;

namespace SymphonyScramble.StateMachine.States.OnGroundStates;

public class EnemyWalkState : EnemyOnGroundState
{
    public EnemyWalkState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Update()
    {

        int i = _stateMachine.Enemy._isPlatform ? _stateMachine.Enemy.Follow(false) : _stateMachine.Enemy.Follow(Globals.CurrentLevel.Player.Position, Globals.CurrentLevel.Player.IsOnGround);

        Vector2 velocity = Vector2.Zero;

        switch (i)
        {
            case 0:
                _stateMachine.TransitionToState(new EnemyIdleState(_stateMachine));
                return;
            case 1:
                velocity.X += _stateMachine.Enemy.Speed;
                break;
            case 2:
                velocity.X -= _stateMachine.Enemy.Speed;
                break;
            case 3:
                _stateMachine.TransitionToState(new EnemyJumpState(_stateMachine));
                break;
            default:
                break;
        }

        _stateMachine.Enemy.Velocity = velocity;

        _stateMachine.Enemy.Move();

        bool isBlocked = _stateMachine.Enemy.HandleCollisionsNonPhys();
        if (isBlocked)
        {
            if (_stateMachine.Enemy._isPlatform)
            {
                _stateMachine.Enemy.Follow(true);
                _stateMachine.TransitionToState(new EnemyWalkState(_stateMachine));
            }

            else _stateMachine.TransitionToState(new EnemyJumpState(_stateMachine));
        }

        // If not on the ground, start falling
        if (!_stateMachine.Enemy.IsOnGround)
        {
            _stateMachine.TransitionToState(new EnemyFallState(_stateMachine));
            return;
        }

        base.Update();
    }
}
