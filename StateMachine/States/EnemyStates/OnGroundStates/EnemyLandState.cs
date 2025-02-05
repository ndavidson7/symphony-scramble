using SymphonyScramble.StateMachine.States.InAirStates;

namespace SymphonyScramble.StateMachine.States.OnGroundStates;

public class EnemyLandState : EnemyOnGroundState
{
    private float _timer;

    public EnemyLandState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
        _timer = 0.08f;
    }

    public override void Update()
    {
        _timer -= (float)Globals.ElapsedSeconds;
        if (_timer <= 0)
        {
            int i = _stateMachine.Enemy.Follow(Globals.CurrentLevel.Player.Position);

            // If move left or right
            switch (i)
            {
                case 1:
                    _stateMachine.TransitionToState(new EnemyWalkState(_stateMachine));
                    break;
                case 2:
                    _stateMachine.TransitionToState(new EnemyWalkState(_stateMachine));
                    break;
                default:
                    _stateMachine.TransitionToState(new EnemyIdleState(_stateMachine));
                    break; 
            }
        }

        base.Update();
    }
}
