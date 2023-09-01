using System.Reflection.PortableExecutable;
using SymphonyScramble.StateMachine.States.InAirStates;

namespace SymphonyScramble.StateMachine.States.OnGroundStates;

public class EnemyIdleState : EnemyOnGroundState
{
    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.Enemy.Velocity = Vector2.Zero;
    }

    public override void Update()
    {

        int i = _stateMachine.Enemy._isPlatform ? _stateMachine.Enemy.Follow(false): _stateMachine.Enemy.Follow(Globals.CurrentLevel.Player.Position, Globals.CurrentLevel.Player.IsOnGround);

        // Find out how it's moving

        // If move left or right
        if (i == 1 || i == 2)
        {
            _stateMachine.TransitionToState(new EnemyWalkState(_stateMachine));
            return;
        }

        if (i == 3)
        {
            _stateMachine.TransitionToState(new EnemyJumpState(_stateMachine));
            return;
        }


        bool isBlocked = _stateMachine.Enemy.HandleCollisionsNonPhys();
        if (isBlocked)
        {
            if (_stateMachine.Enemy._isPlatform)
            {
                _stateMachine.Enemy.Follow(true);
                _stateMachine.TransitionToState(new EnemyWalkState(_stateMachine));
            }
           
           else  _stateMachine.TransitionToState(new EnemyJumpState(_stateMachine));
        }


        base.Update();
    }
}
