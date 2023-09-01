using SymphonyScramble.StateMachine.States.InAirStates;
using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble;

public class EnemyStateMachine
{
    #region Fields
    private readonly Nonphysical _enemy;
    private EnemyState _currentState;
    #endregion

    #region Properties
    public Nonphysical Enemy => _enemy;
    public EnemyState CurrentState => _currentState;

    /* 
     * The "static" keyword effectively makes these objects singletons, meaning only one object will exist for each state.
     * Only need one object for each state since there is only one player. However, this means we must be careful to reset
     * any state-specific fields upon exiting that state or they will remain the same upon reentering that state.
     * 
     * Note: this might not be the most logical place for these to exist. Reference them in other classes like so: "PlayerStateMachine.Idle"
    */
    public EnemyIdleState Idle { get; private set; }
    public EnemyWalkState Walk { get; private set; }
    public EnemyJumpState Jump { get; private set; }
    public EnemyFallState Fall { get; private set; }
    public EnemyLandState Land { get; private set; }
    public EnemyFlyState Fly { get; private set; }
    #endregion

    public EnemyStateMachine(Nonphysical enemy)
    {
        _enemy = enemy;

        Idle = new(this);
        Walk = new(this);
        Jump = new(this);
        Fall = new(this);
        Land = new(this);
        Fly = new(this);

        _currentState = _enemy is FlyingEnemy ? Fly : Idle;
        _currentState.Enter();
    }

    /// <summary>
    /// This method changes the player's current state.
    /// </summary>
    /// <param name="newState">The new state</param>
    public void TransitionToState(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    /// <summary>
    /// This method updates the current state.
    /// It should be called every frame.
    /// </summary>
    public void Update()
    {
        _currentState?.HandleInput();
        _currentState?.Update();
    }
}
