namespace SymphonyScramble;

public class IdleState : OnGroundState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.Player.Velocity = Vector2.Zero;
    }

    public override void HandleInput()
    {
        // if only A or only D is pressed, transition to walk state
        if (Globals.KeyboardState.IsKeyDown(Keys.A) && !Globals.KeyboardState.IsKeyDown(Keys.D) || Globals.KeyboardState.IsKeyDown(Keys.D) && !Globals.KeyboardState.IsKeyDown(Keys.A))
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Walk);
            return;
        }
        base.HandleInput();
    }

    public override void Update()
    {
        base.Update();
        Vector2 velocity = _stateMachine.Player.Velocity;
        if (velocity.Y > 0)
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Fall);
            return;
        }
    }
}
