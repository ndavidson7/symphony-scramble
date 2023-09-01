namespace SymphonyScramble;

public abstract class OnGroundState : PlayerState
{
    protected OnGroundState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void HandleInput()
    {
        if (Globals.KeyboardState.IsKeyDown(Keys.W))
        {
            _stateMachine.TransitionToState(PlayerStateMachine.Jump);
            return;
        }
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
