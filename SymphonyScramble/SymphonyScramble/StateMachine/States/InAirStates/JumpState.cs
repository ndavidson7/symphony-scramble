namespace SymphonyScramble;

public class JumpState : InAirState
{
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.Player.Jump();
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
