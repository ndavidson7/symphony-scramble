namespace SymphonyScramble;

public abstract class InAirState : PlayerState
{
    protected InAirState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void HandleInput()
    {
        Vector2 velocity = _stateMachine.Player.Velocity;
        velocity.X = 0;

        if (Globals.KeyboardState.IsKeyDown(Keys.A))
            velocity.X -= _stateMachine.Player.Speed;
        if (Globals.KeyboardState.IsKeyDown(Keys.D))
            velocity.X += _stateMachine.Player.Speed;

        _stateMachine.Player.Velocity = velocity;
    }

    public override void Update()
    {
        // Add gravity
        _stateMachine.Player.AddGravity();

        // Update position
        _stateMachine.Player.Move();

        // Handle collisions
        _stateMachine.Player.HandleCollisions();
    }
}
