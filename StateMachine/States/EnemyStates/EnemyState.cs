namespace SymphonyScramble;

/// <summary>
/// PlayerState is an abstract representation of a state.
/// To implement a new state, it must inherit from this class at some point in its hierarchy.
/// (States can have parent states that inherit from this, e.g., idle, walking, and landing states
/// all inherit farom a base OnGroundState that inherits from PlayerState.)
/// 
/// Because all methods are virtual except HandleInput(), only HandleInput() MUST be implemented
/// in the new state. However, the default for every other method is to do nothing, so these will
/// often be reimplemented in concrete states by using the "override" keyword.
/// </summary>
public abstract class EnemyState
{
    protected EnemyStateMachine _stateMachine;

    protected EnemyState(EnemyStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    /// <summary>
    /// This method is called upon entering the state.
    /// 
    /// Not all states need custom entering logic. An example of a state that does
    /// is jumping: upon entering, the player should be given an immediate upward boost
    /// of velocity.
    /// </summary>
    public virtual void Enter() { return; }

    /// <summary>
    /// This method is called upon exiting the state.
    /// 
    /// As with Enter(), not all states need custom exiting logic. An example of a state
    /// that does is falling: upon exiting, the player's Y velocity should be reset to zero.
    /// </summary>
    public virtual void Exit()
    {
        _stateMachine.Enemy.ResetAnimation();
    }

    /// <summary>
    /// This method defines the inputs that are allowed in the state and how they should
    /// be handled.
    /// 
    /// As far as I can imagine, every state should handle SOME inputs, but this may not
    /// be true of our end product.
    /// </summary>
    public virtual void HandleInput() { return; }

    /// <summary>
    /// This method handles the bulk of the business logic for the state.
    /// 
    /// Statements that should be run every MonoGame frame, regardless of player input,
    /// should be written here.
    /// </summary>
    public virtual void Update() { return; }
}

