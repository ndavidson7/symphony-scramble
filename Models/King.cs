namespace SymphonyScramble;

public class King : Actor
{
    // hard-coded, king-specific numbers for bounding box and hitbox
    private const int UNSCALED_OFFSET_X = 0;
    private const int UNSCALED_OFFSET_Y = 0;
    private const int UNSCALED_WIDTH = 8;
    private const int UNSCALED_HEIGHT = 8;
    // defaults in case not specified in constructor call
    private const float DEFAULT_SCALE = 1;
    private const float DEFAULT_SPEED = 100;
    private const float DEFAULT_JUMPFORCE = 200;
    private const float DEFAULT_GRAVITY = 12;
    //private const float DEFAULT_HITFORCE = 300;
    private const float FASTFALL_GRAVITY = 20;

    public static float DefaultGravity => DEFAULT_GRAVITY;
    public static float FastFallGravity => FASTFALL_GRAVITY;

    private Texture2D _idleTexture;
    private Texture2D _walkTexture;
    private Texture2D _jumpTexture;
    private Texture2D _fallTexture;
    private Texture2D _landTexture;

    public King(Vector2 position, float scale = DEFAULT_SCALE, float speed = DEFAULT_SPEED, float jumpForce = DEFAULT_JUMPFORCE, float gravity = DEFAULT_GRAVITY)
        : base(position, scale, speed, jumpForce, gravity, UNSCALED_OFFSET_X, UNSCALED_OFFSET_Y, UNSCALED_WIDTH, UNSCALED_HEIGHT) { }

    public override void LoadContent()
    {
        _idleTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/Idle");
        _walkTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/Run");
        _jumpTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/Jump");
        _fallTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/Fall");
        _landTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/Land");
    }

    public override void AddAnims()
    {
        _anims.AddAnimation(PlayerStateMachine.Idle, new Animation(_idleTexture, 2, 1, 0.5f, scale: _scale));
        _anims.AddAnimation(PlayerStateMachine.Walk, new Animation(_walkTexture, 5, 1, 0.08f, scale: _scale));
        _anims.AddAnimation(PlayerStateMachine.Jump, new Animation(_jumpTexture, 1, 1, 0, scale: _scale, looping: false));
        _anims.AddAnimation(PlayerStateMachine.Fall, new Animation(_fallTexture, 1, 1, 0, scale: _scale, looping: false));
        _anims.AddAnimation(PlayerStateMachine.Land, new Animation(_landTexture, 1, 1, 0, scale: _scale, looping: false));
    }
}

