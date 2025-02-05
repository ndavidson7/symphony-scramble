namespace SymphonyScramble;

public class BouncyPlatform : Tile
{
    private const float DEFAULT_SCALE = 1f;
    private const int UNSCALED_WIDTH = 8;
    private const int UNSCALED_HEIGHT = 8;

    public static int UnscaledWidth => UNSCALED_WIDTH;
    public static int UnscaledHeight => UNSCALED_HEIGHT;

    public BouncyPlatform(Vector2 position, float scale = DEFAULT_SCALE) : base(position, UNSCALED_WIDTH, UNSCALED_HEIGHT, scale)
    {
    }
}
