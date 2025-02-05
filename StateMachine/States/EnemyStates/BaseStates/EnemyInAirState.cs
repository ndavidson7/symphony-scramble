namespace SymphonyScramble;

public abstract class EnemyInAirState : EnemyState
{
    protected EnemyInAirState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Update()
    {
        int i = _stateMachine.Enemy.Follow(Globals.CurrentLevel.Player.Position);

        Vector2 velocity = _stateMachine.Enemy.Velocity;
        velocity.X = 0;

        switch (i)
        {
            case 1:
                velocity.X += _stateMachine.Enemy.Speed;
                break;
            case 2:
                velocity.X -= _stateMachine.Enemy.Speed;
                break;
            default:
                break;
        }

        _stateMachine.Enemy.Velocity = velocity;

        // Add gravity
        _stateMachine.Enemy.AddGravity();

        // Update position
        _stateMachine.Enemy.Move();

        // Handle collisions
        _stateMachine.Enemy.HandleCollisionsNonPhys();
    }
}
