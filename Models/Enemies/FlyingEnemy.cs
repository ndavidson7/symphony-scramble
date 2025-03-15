using System;
using Microsoft.Xna.Framework.Graphics;
using SymphonyScramble.Models;
using SymphonyScramble.StateMachine.States.InAirStates;
using SymphonyScramble.StateMachine.States.OnGroundStates;

namespace SymphonyScramble
{
    public class FlyingEnemy : Nonphysical
    {

        private static Rectangle SOURCE_RECTANGLE = new(0, 0, 32, 32);
        private const float DEFAULT_SCALE = 1f;
        private const float DEFAULT_GRAVITY = 12;
        private const float DEFAULT_SPEED = 20;
        private const float DEFAULT_JUMPFORCE = 200;

        private const int UNSCALED_OFFSET_X = 0;
        private const int UNSCALED_OFFSET_Y = 0;
        private const int UNSCALED_WIDTH = 8;
        private const int UNSCALED_HEIGHT = 8;

        private const int DIFFICULTY_SPEED_EASY = 10;
        private const int DIFFICULTY_SPEED_MEDIUM = 35;
        private const int DIFFICULTY_SPEED_HARD = 60;



        private Texture2D _flyTexture;


        public FlyingEnemy(Vector2 position, float jumpForce = DEFAULT_JUMPFORCE, float scale = DEFAULT_SCALE, float gravity = DEFAULT_GRAVITY) :
            base(position, true, scale, SOURCE_RECTANGLE, DEFAULT_JUMPFORCE, gravity, UNSCALED_OFFSET_X, UNSCALED_OFFSET_Y, UNSCALED_WIDTH, UNSCALED_HEIGHT)
        {
        }


        public override void LoadContent()
        {

            _flyTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/CharAndEnemy/FlyingEnemy");
 
            //_attackTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/PlatformEnemy/Attack");
        }


        public override void AddAnims()
        {
            _anims.AddAnimation(new EnemyFlyState(_stateMachine), new Animation(_flyTexture, 4, 1, .1f, scale: _scale));
            // _anims.AddAnimation(new EnemyAttackState(_stateMachine), new Animation(_attackTexture, 3, 1, 0.15f, scale: _scale, looping: false));
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

