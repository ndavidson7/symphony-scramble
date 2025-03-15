namespace SymphonyScramble.Models;

public class LevelEnd : ICollidable
{
    private readonly BoundingRectangle _bounds;

    public BoundingRectangle Bounds => _bounds;

    public LevelEnd(Vector2 position, Vector2 dimensions)
    {
        _bounds = new(position.X, position.Y, dimensions.X, dimensions.Y);
    }
}
