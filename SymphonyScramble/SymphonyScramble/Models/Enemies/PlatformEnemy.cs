using System;
using Microsoft.Xna.Framework.Graphics;
using SymphonyScramble.StateMachine.States.InAirStates;
using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble
{
    public class PlatformEnemy : Nonphysical
    {

        private static Rectangle SOURCE_RECTANGLE = new(0, 0, 32, 32);
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

        
       

        public PlatformEnemy(Vector2 position, float jumpForce = DEFAULT_JUMPFORCE, float scale = DEFAULT_SCALE, float gravity = DEFAULT_GRAVITY) :
            base(position, true, scale, SOURCE_RECTANGLE, DEFAULT_JUMPFORCE, gravity, UNSCALED_OFFSET_X, UNSCALED_OFFSET_Y, UNSCALED_WIDTH, UNSCALED_HEIGHT)
        {
        }

   

        public override void LoadContent()
        {

            _idleTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Idle");
            _walkTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Run");
            _jumpTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Jump");
            _fallTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Fall");
            _landTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Land");
            //_attackTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Attack");
        }


        public override void AddAnims()
        {
            _anims.AddAnimation(new EnemyIdleState(_stateMachine), new Animation(_idleTexture, 8, 1, 0.5f, scale: _scale));
            _anims.AddAnimation(new EnemyWalkState(_stateMachine), new Animation(_walkTexture, 6, 1, 0.15f, scale: _scale));
            _anims.AddAnimation(new EnemyJumpState(_stateMachine), new Animation(_jumpTexture, 2, 1, 0, scale: _scale, looping: false));
            _anims.AddAnimation(new EnemyFallState(_stateMachine), new Animation(_fallTexture, 1, 1, 0, scale: _scale, looping: false));
            _anims.AddAnimation(new EnemyLandState(_stateMachine), new Animation(_landTexture, 2, 1, 0, scale: _scale, looping: false));
           // _anims.AddAnimation(new EnemyAttackState(_stateMachine), new Animation(_attackTexture, 3, 1, 0.15f, scale: _scale, looping: false));
        }

    }
}

