using SymphonyScramble.Animations;

namespace SymphonyScramble.Models;

public abstract class Actor : Sprite
{
    // Handle bulk of Actor logic
    protected readonly PlayerStateMachine _stateMachine;
    protected readonly AnimationManager _anims;

    // Actor subclass-specific bounding fields
    protected readonly int _offsetX; // offset in pixels from _position.X to start of bounding box
    protected readonly int _offsetY; // offset in pixels from _position.Y to start of bounding box
    protected readonly int _width; // width of bounding box in pixels, not the same as frame width due to transparent pixels
    protected readonly int _height; // height of bounding box

    // Can be updated by powerups, etc.
    protected float _speed; // movement speed
    protected float _jumpForce; // determines initial upwards velocity when jumping
    protected float _gravity; // pull of gravity on actor

    // Determines whether Actor should be drawn horizontally flipped
    private bool _lastFlip = false;

    public bool isHurt;
    public float hurtTimer;

    public Actor(Vector2 position, float scale, float speed, float jumpForce, float gravity, int UNSCALED_OFFSET_X, int UNSCALED_OFFSET_Y, int UNSCALED_WIDTH, int UNSCALED_HEIGHT) : base(position, scale)
    {
        _stateMachine = new PlayerStateMachine(this);
        _anims = new AnimationManager();
        _offsetX = (int)(scale * UNSCALED_OFFSET_X);
        _offsetY = (int)(scale * UNSCALED_OFFSET_Y);
        _width = (int)(scale * UNSCALED_WIDTH);
        _height = (int)(scale * UNSCALED_HEIGHT);
        _speed = speed;
        _jumpForce = jumpForce;
        _gravity = gravity;
        hurtTimer = 0f;

        AddAnims();
    }

    public override BoundingRectangle Bounds { get { return new BoundingRectangle(_position.X + _offsetX, _position.Y + _offsetY, _width, _height); } }

    public override Vector2 Center { get { return new Vector2(_position.X + _offsetX + _width / 2.0f, _position.Y + _offsetY + _height / 2.0f); } }

    public float Speed { get => _speed; set => _speed = value; }

    public float JumpForce { get => _jumpForce; set => _jumpForce = value; }

    public float Gravity { get => _gravity; set => _gravity = value; }

    public PlayerStateMachine StateMachine => _stateMachine;

    public override void Update()
    {
        if (isHurt) CheckHitTimer();
        _stateMachine.Update();
        _anims.Update(_stateMachine.CurrentState);
    }

    public override void Draw()
    {
        //if (Globals.KeyboardState.IsKeyDown(Keys.O))
        //{
        //    Globals.SpriteBatch.Draw(Globals.DebugTexture, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.White);
        //}
        if (Globals.KeyboardState.IsKeyDown(Keys.A) && !Globals.KeyboardState.IsKeyDown(Keys.D)) _lastFlip = true;
        else if (Globals.KeyboardState.IsKeyDown(Keys.D) && !Globals.KeyboardState.IsKeyDown(Keys.A)) _lastFlip = false;
        _anims.Draw(_position, _lastFlip);
    }

    public override void KeepInBounds()
    {
        _position = Vector2.Clamp(_position, Vector2.Zero - new Vector2(_offsetX, _offsetY), new Vector2(Config.WindowSize.X - _width - _offsetX, Config.WindowSize.Y - _height - _offsetY));
    }

    public abstract void AddAnims();

    public bool IsAnimFinished()
    {
        return _anims.IsAnimFinished();
    }

    public void ResetAnimation()
    {
        _anims.ResetAnimation();
    }

    public void AddGravity()
    {
        _velocity.Y += _gravity;
    }

    public void CheckHitTimer()
    {
        hurtTimer -= (float)Globals.ElapsedSeconds;
        if (hurtTimer <= 0)
        {
            isHurt = false;
        }
    }

    public void Jump()
    {
        // Make actor jump
        _velocity.Y -= _jumpForce;
    }
    public void EnemyBounce()
    {
        // Make actor Bounce
        _velocity.Y -= _jumpForce;
    }

    public void Bounce()
    {
        // Make actor Bounce
        _velocity.Y -= _jumpForce * 1.5f;
    }
    public void GetHit()
    {
        isHurt = true;
        hurtTimer = 2f;
    }

    public void CheckOnGround(ICollidable ground)
    {
        if (ground == null) return;

        if (ground is TimedPlatform)
        {
            var newGround = (TimedPlatform)ground;
            if (newGround.Removed && IsTouchingTop(newGround))
            {
                _lastGround = null;
                _stateMachine.TransitionToState(PlayerStateMachine.Fall);
            }
        }
        else if (!IsTouchingTop(ground))
        {
            _lastGround = null;
            _stateMachine.TransitionToState(PlayerStateMachine.Fall);
        }
    }

    public bool InBounds(Vector2 bounds)
    {
        return _position.X > 0 &&
               _position.Y > 0 &&
               _position.X <= bounds.X &&
               _position.Y <= bounds.Y;
    }

    //public void HandleHits()
    //{
    //    foreach (Interactable o in Globals.CurrentLevel.Interactables)
    //    {
    //        if (Hitbox.CollidesWith(o.Bounds))
    //        {
    //            Vector2 direction = Vector2.Normalize(o.Center - Center);
    //            o.Velocity += direction * _hitForce;
    //        }
    //    }
    //}
}
