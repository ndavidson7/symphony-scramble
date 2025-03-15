using SymphonyScramble.Models;

namespace SymphonyScramble;

public class FallState : InAirState
{
    public FallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Vector2 velocity = _stateMachine.Player.Velocity;
        velocity.Y = 0;
        _stateMachine.Player.Velocity = velocity;
    }

    public override void Exit()
    {
        base.Exit();

        Vector2 velocity = _stateMachine.Player.Velocity;
        velocity.Y = 0;
        _stateMachine.Player.Velocity = velocity;
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.Player.IsOnBounce)
        {
            _stateMachine.Player.IsOnBounce = false;
            _stateMachine.TransitionToState(PlayerStateMachine.Bounce);
            return;
        }

        if (_stateMachine.Player.IsOnEnemyBounce)
        {
            _stateMachine.Player.IsOnEnemyBounce = false;
            _stateMachine.TransitionToState(PlayerStateMachine.EnemyBounce);
            return;
        }
        // Check if landed on ground
        if (_stateMachine.Player.IsOnGround)
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Land);
            return;
        }
    }

    public override void HandleInput()
    {
        Vector2 velocity = _stateMachine.Player.Velocity;
        velocity.X = 0;

        if (Globals.KeyboardState.IsKeyDown(Keys.A))
            velocity.X -= _stateMachine.Player.Speed;
        if (Globals.KeyboardState.IsKeyDown(Keys.D))
            velocity.X += _stateMachine.Player.Speed;
        if (Globals.KeyboardState.IsKeyDown(Keys.S))
        {
            _stateMachine.Player.Gravity = Player.FastFallGravity;
        }
        else
        {
            _stateMachine.Player.Gravity = Player.DefaultGravity;
        }
        _stateMachine.Player.Velocity = velocity;
    }
}