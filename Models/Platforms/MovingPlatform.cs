namespace SymphonyScramble;

public class MovingPlatform : SolidPlatform
{
    private const float DEFAULT_SCALE = 1f;
    private const int UNSCALED_WIDTH = 8;
    private const int UNSCALED_HEIGHT = 8;

    private Texture2D platformSprite;

    private int distanceTimer;
    private bool direction; // left = false, right = true
    private bool touchPlayer;

    public MovingPlatform(Vector2 position, float scale = DEFAULT_SCALE) : base(position, scale)
    {
        distanceTimer = 0;
        direction = true;
        touchPlayer = false;
    }

    public override void Update()
    {
        if (!(Globals.CurrentLevel.Player.IsOnGround))
        {
            SetTouched(false);
        }
        if (direction)
        {
            ChangeBounds(1, 0);
            if (touchPlayer && (Globals.CurrentLevel.Player.StateMachine.CurrentState is IdleState))
            {
                Globals.CurrentLevel.Player.Position += new Vector2(1, 0);
            }
            distanceTimer++;
        }
        else
        {
            ChangeBounds(-1, 0);
            if (touchPlayer && (Globals.CurrentLevel.Player.StateMachine.CurrentState is IdleState))
            {
                Globals.CurrentLevel.Player.Position -= new Vector2(1, 0);
            }
            distanceTimer++;
        }
        if (distanceTimer >= 45)
        {
            direction = !direction;
            distanceTimer = 0;
        }
    }

    public void SetTouched(bool b)
    {
        touchPlayer = b;
    }

    public void Load()
    {
        platformSprite = Globals.Content.Load<Texture2D>("Sprites/Platforms/moving");
    }

    public override void Draw()
    {
        int number = (int)Bounds.Width / UNSCALED_WIDTH;
        for (int i = 0; i < number; i++)
        {
            Globals.SpriteBatch.Draw(platformSprite, new Rectangle((int)Bounds.X + i * UNSCALED_WIDTH, (int)Bounds.Y, UNSCALED_WIDTH, UNSCALED_HEIGHT), Color.White);
        }
    }
}

