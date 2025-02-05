using System;
namespace SymphonyScramble;

public class TimedPlatform : SemiSolidPlatform
{
    private const float DEFAULT_SCALE = 1f;
    private const int UNSCALED_WIDTH = 8;
    private const int UNSCALED_HEIGHT = 8;

    private bool _hasBeenTouched = false;
    private double _timer = 3;

    private Texture2D platformSprite;

    public bool Removed => _timer <= 0;

    public TimedPlatform(Vector2 position, float scale = DEFAULT_SCALE) : base(position, scale)
    {
    }

    public override void Update()
    {
        if (_hasBeenTouched)
        {
            _timer -= Globals.ElapsedSeconds * 2;
        }

        if (_timer <= 0)
        {
            Globals.CurrentLevel.toRemove = this;
        }
    }

    public void Load()
    {
        platformSprite = Globals.Content.Load<Texture2D>("Sprites/Platforms/timed");
    }

    public void SetTouched()
    {
        _hasBeenTouched = true;
    }

    public override void Draw()
    {
        if (_timer <= 3 && _timer > 2) {
            Globals.SpriteBatch.Draw(platformSprite, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.White);
        }
        else if (_timer <= 2 && _timer > 1)
        {
            Globals.SpriteBatch.Draw(platformSprite, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.Orange);
        }
        else if (_timer <= 1 && _timer > 0)
        {
            Globals.SpriteBatch.Draw(platformSprite, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.Red);
        }
    }
}

