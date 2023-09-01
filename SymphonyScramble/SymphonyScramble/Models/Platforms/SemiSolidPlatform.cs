﻿namespace SymphonyScramble;

public class SemiSolidPlatform : Tile
{
    private const float DEFAULT_SCALE = 1f;
    private const int UNSCALED_WIDTH = 8;
    private const int UNSCALED_HEIGHT = 8;

    public static int UnscaledWidth = UNSCALED_WIDTH;
    public static int UnscaledHeight = UNSCALED_HEIGHT;

    public SemiSolidPlatform(Vector2 position, float scale = DEFAULT_SCALE) : base(position, UNSCALED_WIDTH, UNSCALED_HEIGHT, scale)
    {
    }
}
