namespace SymphonyScramble;

public abstract class EnemyOnGroundState : EnemyState
{
    protected EnemyOnGroundState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }


    public override void Update()
    {
        _stateMachine.Enemy.HandleCollisionsNonPhys();
    }
}
