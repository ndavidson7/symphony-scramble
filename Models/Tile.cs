using Microsoft.Xna.Framework.Graphics;

namespace SymphonyScramble;

public abstract class Tile : ICollidable
{
    private float _scale;
    private BoundingRectangle _bounds;
    private bool _isGround = false;
    private bool _isWall = false;

    public BoundingRectangle Bounds => _bounds;
    public bool IsGround => _isGround;
    public bool IsWall => _isWall;

    public Tile(Vector2 position, int width, int height, float scale)
    {
        _scale = scale;
        _bounds = new BoundingRectangle(position.X, position.Y, width * scale, height * scale);
    }

    public virtual void Update()
    {
        // no updating necessary for fixed tiles
        return;
    }

    public virtual void Draw()
    {
        Globals.SpriteBatch.Draw(Globals.DebugTexture, new Rectangle((int)_bounds.X, (int)_bounds.Y, (int)_bounds.Width, (int)_bounds.Height), Color.White);
    }

    public void ChangeBounds(int x, int y)
    {
        _bounds.X += x;
        _bounds.Y += y;
    }

    public void ExtendRight(int width)
    {
        _bounds.Width += width * _scale;
        _isGround = true;
    }

    public void ExtendDown(int height)
    {
        _bounds.Height += height * _scale;
        _isWall = true;
    }

    public void ExtendLeft(int width)
    {
        var extension = width * _scale;
        _bounds.X -= extension;
        _bounds.Width += extension;
    }

    public void ExtendUp(int height)
    {
        var extension = height * _scale;
        _bounds.Y -= extension;
        _bounds.Height += extension;
    }
}
