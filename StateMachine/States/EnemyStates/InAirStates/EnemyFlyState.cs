using System;
using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble
{
	public class EnemyFlyState: EnemyInAirState
	{
        public EnemyFlyState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Update()
        {
            // Set direction
            _stateMachine.Enemy.Fly(Globals.CurrentLevel.Player.Position);

            // Update position
            _stateMachine.Enemy.Move();

            // Handle collisions
            _stateMachine.Enemy.HandleCollisionBird();
        }
    }
}

