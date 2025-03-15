using SymphonyScramble.Models;

namespace SymphonyScramble;

public class LandState : OnGroundState
{
    private float _timer;

    public LandState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        _timer = 0.08f;
    }

    public override void Enter()
    {
        Player k = (Player)_stateMachine.Player;
        _stateMachine.Player.Gravity = Player.DefaultGravity;
    }

    public override void HandleInput()
    {
        if (_timer <= 0)
        {
            // should be guaranteed to transition out of state now, so reset timer
            _timer = 0.01f;

            if (Globals.KeyboardState.GetPressedKeyCount() == 0)
            {
                _stateMachine.TransitionToState(PlayerStateMachine.Idle);
                return;
            }

            if (Globals.KeyboardState.IsKeyDown(Keys.A) || Globals.KeyboardState.IsKeyDown(Keys.D))
            {
                _stateMachine.TransitionToState(PlayerStateMachine.Walk);
            }

            base.HandleInput();
        }
    }

    public override void Update()
    {
        _timer -= (float)Globals.ElapsedSeconds;
    }
}
