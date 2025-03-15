using SymphonyScramble.Models;

namespace SymphonyScramble;

public class PlayerStateMachine
{
    #region Fields
    private readonly Actor _player;
    private PlayerState _currentState;
    #endregion

    #region Properties
    public Actor Player => _player;
    public PlayerState CurrentState => _currentState;

    /* 
     * The "static" keyword effectively makes these objects singletons, meaning only one object will exist for each state.
     * Only need one object for each state since there is only one player. However, this means we must be careful to reset
     * any state-specific fields upon exiting that state or they will remain the same upon reentering that state.
     * 
     * Note: this might not be the most logical place for these to exist. Reference them in other classes like so: "PlayerStateMachine.Idle"
    */
    public static IdleState Idle { get; private set; }
    public static WalkState Walk { get; private set; }
    public static JumpState Jump { get; private set; }
    public static FallState Fall { get; private set; }
    public static LandState Land { get; private set; }
    // public static AttackState Attack { get; private set; }
    public static BounceState Bounce { get; private set; }
    public static BounceOnEnemyState EnemyBounce { get; private set; }


    #endregion

    public PlayerStateMachine(Actor player)
    {
        _player = player;

        Idle = new(this);
        Walk = new(this);
        Jump = new(this);
        Fall = new(this);
        Land = new(this);
        // Attack = new(this);
        Bounce = new(this);
        EnemyBounce = new(this);

        _currentState = Idle;
        _currentState.Enter();
    }

    /// <summary>
    /// This method changes the player's current state.
    /// </summary>
    /// <param name="newState">The new state</param>
    public void TransitionToState(PlayerState newState)
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
