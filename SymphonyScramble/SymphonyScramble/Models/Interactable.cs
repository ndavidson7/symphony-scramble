using static System.Math;

namespace SymphonyScramble;

public abstract class Interactable : Sprite
{
    protected Rectangle _sourceRectangle;
    protected float _gravity;
    protected float _pushDrag;
    protected float _hitDrag;

    public override BoundingRectangle Bounds
    {
        get
        {
            return new BoundingRectangle(_position.X, _position.Y, _sourceRectangle.Width * _scale, _sourceRectangle.Height * _scale);
        }
    }
    public float PushDrag => _pushDrag;

    public Interactable(Vector2 position, float scale, Rectangle sourceRectangle, float gravity, float pushDrag, float hitDrag) : base(position, scale)
    {
        _sourceRectangle = sourceRectangle;
        _gravity = gravity;
        _pushDrag = pushDrag;
        _hitDrag = hitDrag;
    }

    public override void Update()
    {
        if (!_isOnGround) AddGravity();
        Move();
        if (_isOnGround) AddDrag();
        HandleCollisions();
    }

    public void AddGravity()
    {
        _velocity.Y += _gravity;
    }

    public void AddDrag()
    {
        _velocity.X = Abs(_velocity.X) > 1f ? _velocity.X * _hitDrag : 0f;
    }
}
