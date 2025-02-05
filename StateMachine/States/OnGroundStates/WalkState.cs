namespace SymphonyScramble;

public class WalkState : OnGroundState
{
    public WalkState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void HandleInput()
    {
        // if neither A or D is pressed OR both A and D are pressed, transition to idle
        if (!(Globals.KeyboardState.IsKeyDown(Keys.A) || Globals.KeyboardState.IsKeyDown(Keys.D)) || Globals.KeyboardState.IsKeyDown(Keys.A) && Globals.KeyboardState.IsKeyDown(Keys.D))
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Idle);
            return;
        }

        Vector2 velocity = Vector2.Zero;

        if (Globals.KeyboardState.IsKeyDown(Keys.A))
            velocity.X -= _stateMachine.Player.Speed;
        if (Globals.KeyboardState.IsKeyDown(Keys.D))
            velocity.X += _stateMachine.Player.Speed;

        _stateMachine.Player.Velocity = velocity;

        base.HandleInput();
    }

    public override void Update()
    {
        _stateMachine.Player.Move();

        _stateMachine.Player.HandleCollisions();

        // If not on the ground, start falling
        if (!_stateMachine.Player.IsOnGround)
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Fall);
            return;
        }
    }
}
