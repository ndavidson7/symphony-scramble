using System;
using Microsoft.Xna.Framework.Graphics;
using SymphonyScramble.StateMachine.States.InAirStates;
using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble
{
    public class FollowingEnemy : Nonphysical
    {

        private static Rectangle SOURCE_RECTANGLE = new(0, 0, 8, 8);
        private const float DEFAULT_SCALE = 1f;
        private const float DEFAULT_GRAVITY = 12;
        private const float DEFAULT_JUMPFORCE = 200;

        private const int UNSCALED_OFFSET_X = 0;
        private const int UNSCALED_OFFSET_Y = 0;
        private const int UNSCALED_WIDTH = 8;
        private const int UNSCALED_HEIGHT = 8;

        private Texture2D _idleTexture;
        private Texture2D _walkTexture;
        private Texture2D _jumpTexture;
        private Texture2D _fallTexture;
        private Texture2D _landTexture;
        private Texture2D _attackTexture;

        private const int DIFFICULTY_SPEED_EASY = 10;
        private const int DIFFICULTY_SPEED_MEDIUM = 30;
        private const int DIFFICULTY_SPEED_HARD = 40;


        public FollowingEnemy(Vector2 position, float jumpForce = DEFAULT_JUMPFORCE, float scale = DEFAULT_SCALE, float gravity = DEFAULT_GRAVITY) :
            base(position,false,  scale, SOURCE_RECTANGLE, DEFAULT_JUMPFORCE, gravity, UNSCALED_OFFSET_X, UNSCALED_OFFSET_Y, UNSCALED_WIDTH, UNSCALED_HEIGHT)
        {
        }

        public override void LoadContent()
        {
            
                _idleTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/ChasingEnemy/Idle");
                _walkTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/ChasingEnemy/Run");
                _jumpTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/ChasingEnemy/Jump");
                _fallTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/ChasingEnemy/Fall");
                _landTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/ChasingEnemy/Land");

        }


        public override void AddAnims()
        {
            _anims.AddAnimation(new EnemyIdleState(_stateMachine), new Animation(_idleTexture, 8, 1, 0.5f, scale: _scale));
            _anims.AddAnimation(new EnemyWalkState(_stateMachine), new Animation(_walkTexture, 6, 1, 0.15f, scale: _scale)); 
            _anims.AddAnimation(new EnemyJumpState(_stateMachine), new Animation(_jumpTexture, 2, 1, 0, scale: _scale, looping: false));
            _anims.AddAnimation(new EnemyFallState(_stateMachine), new Animation(_fallTexture, 1, 1, 0, scale: _scale, looping: false));
            _anims.AddAnimation(new EnemyLandState(_stateMachine), new Animation(_landTexture, 2, 1, 0, scale: _scale, looping: false));
            //_anims.AddAnimation(new EnemyAttackState(_stateMachine), new Animation(_attackTexture, 3, 1, 0.15f, scale: _scale, looping: false));
        }

        public override int determineSpeed()
        {
            switch (Globals.CurrentLevel._difficulty)
            {
                case ("EASY"):
                    return DIFFICULTY_SPEED_EASY;
                case ("HARD"):
                    return DIFFICULTY_SPEED_HARD;
                default:
                    return DIFFICULTY_SPEED_MEDIUM;
            }
        }

    }
}

