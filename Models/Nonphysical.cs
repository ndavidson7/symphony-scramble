using SymphonyScramble.Animations;

namespace SymphonyScramble.Models;

public abstract class Nonphysical : Sprite
{

    protected EnemyStateMachine _stateMachine;
    protected AnimationManager _anims;


    protected Rectangle _sourceRectangle;
    protected float _speed; // movement speed
    protected float _gravity;
    protected float _jumpforce;
    protected bool _hasBeenTouched;
    protected bool isBlocked;

    protected readonly int _offsetY; // offset in pixels from _position.Y to start of bounding box
    protected readonly int _offsetX;
    protected readonly int _width; // width of bounding box in pixels, not the same as frame width due to transparent pixels
    protected readonly int _height; // height of bounding box


    protected bool _lastFlip = false;


    public bool _isPlatform = false;


    private const int DIFFICULTY_SPEED_EASY = 10;
    private const int DIFFICULTY_SPEED_MEDIUM = 40;
    private const int DIFFICULTY_SPEED_HARD = 80;

    public float Speed { get => _speed; set => _speed = value; }
    public override BoundingRectangle Bounds { get { return new BoundingRectangle(_position.X + _offsetX, _position.Y + _offsetY, _width, _height); } }
    public override Vector2 Center { get { return new Vector2(_position.X + _offsetX + _width / 2.0f, _position.Y + _offsetY + _height / 2.0f); } }
    public Nonphysical(Vector2 position, bool isPlatform, float scale, Rectangle sourceRectangle, float jumpForce, float gravity, int UNSCALED_OFFSET_X,
         int UNSCALED_OFFSET_Y, int UNSCALED_WIDTH, int UNSCALED_HEIGHT) : base(position, scale)
    {
        _anims = new AnimationManager(true);
        _stateMachine = new EnemyStateMachine(this);
        _sourceRectangle = sourceRectangle;
        _gravity = gravity;
        _speed = determineSpeed();
        _jumpforce = jumpForce;
        _offsetX = (int)(scale * UNSCALED_OFFSET_X);
        _offsetY = (int)(scale * UNSCALED_OFFSET_Y);
        _width = (int)(scale * UNSCALED_WIDTH);
        _height = (int)(scale * UNSCALED_HEIGHT);
        _isPlatform = isPlatform;
        AddAnims();
    }

    public virtual int determineSpeed()
    {
        switch (Globals.CurrentLevel._difficulty)
        {
            case "EASY":
                return DIFFICULTY_SPEED_EASY;
            case "HARD":
                return DIFFICULTY_SPEED_HARD;
            default:
                return DIFFICULTY_SPEED_MEDIUM;
        }
    }

    public override void KeepInBounds()
    {
        _position = Vector2.Clamp(_position, Vector2.Zero - new Vector2(_offsetX, _offsetY), new Vector2(Config.WindowSize.X - _width, Config.WindowSize.Y - _height - _offsetY));
    }

    public int Follow(bool isAtEnd)
    {
        if (isAtEnd)
        {
            _lastFlip = !_lastFlip;
            _velocity *= 1;
        }
        if (_lastFlip) return 2;
        return 1;
    }

    public int Follow(Vector2 playerPosition, bool isOnGround)
    {

        if (playerPosition.X > _position.X + 2)
        {
            _lastFlip = false;
            return 1;
        }

        if (playerPosition.X < _position.X - 2)
        {
            _lastFlip = true;
            return 2;
        }

        if (playerPosition.Y < _position.Y - 10 && isOnGround)
        {

            return 3;
        }

        return 0;
    }

    public int Follow(Vector2 playerPosition)
    {

        if (playerPosition.X > _position.X + 2)
        {
            _lastFlip = false;
            return 1;
        }

        if (playerPosition.X < _position.X - 2)
        {
            _lastFlip = true;
            return 2;
        }

        return 0;
    }

    public void Fly(Vector2 playerPosition)
    {
        Vector2 direction = Vector2.Normalize(playerPosition - _position);
        _velocity = direction * _speed;

        _lastFlip = direction.X < 0;
    }

    public void AddGravity()
    {
        AddVelocityY(_gravity);
    }

    public void Jump()
    {
        AddVelocityY(-_jumpforce);
    }

    public void AddVelocityY(float veloChange)
    {
        _velocity.Y += veloChange;
    }

    public override void Update()
    {
        _stateMachine.Update();
        _anims.Update(_stateMachine.CurrentState);
    }

    public abstract void AddAnims();

    public override void Draw()
    {
        //if (Globals.KeyboardState.IsKeyDown(Keys.Q))
        //{
        //    Globals.SpriteBatch.Draw(Globals.DebugTexture, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.White);
        //}
        _anims.Draw(_position, _lastFlip);
    }

    public bool IsAnimFinished()
    {
        return _anims.IsAnimFinished();
    }

    public void ResetAnimation()
    {
        _anims.ResetAnimation();
    }


}

