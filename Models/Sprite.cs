using SymphonyScramble.Models;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace SymphonyScramble;

public abstract class Sprite : ICollidable
{
    protected Texture2D? _texture;
    protected Vector2 _position;
    protected Vector2 _velocity = Vector2.Zero;
    protected float _scale;
    protected bool _isOnGround = false;
    protected bool _isOnBounce = false;
    protected bool _isOnEnemyBounce = false;
    protected ICollidable? _lastGround;

    public bool IsOnGround => _isOnGround;
    public bool IsOnBounce { get => _isOnBounce; set => _isOnBounce = value; }
    public bool IsOnEnemyBounce { get => _isOnEnemyBounce; set => _isOnEnemyBounce = value; }
    public ICollidable? LastGround => _lastGround;

    public virtual BoundingRectangle Bounds
    {
        get
        {
            return new BoundingRectangle(_position.X, _position.Y, _texture.Width * _scale, _texture.Height * _scale);
        }
    }
    public virtual Vector2 Center { get { return new Vector2(_position.X + Bounds.Width / 2.0f, _position.Y + Bounds.Height / 2.0f); } }
    public Vector2 Position { get => _position; set => _position = value; }
    public Vector2 Velocity { get => _velocity; set => _velocity = value; }

    public Sprite(Vector2 position, float scale)
    {
        _position = position;
        _scale = scale;
        LoadContent();
    }

    public abstract void LoadContent();

    public abstract void Update();

    public virtual void Draw()
    {
        //if (Globals.KeyboardState.IsKeyDown(Keys.L))
        //{
        //    Globals.SpriteBatch.Draw(Globals.DebugTexture, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.White);
        //}
        Globals.SpriteBatch.Draw(_texture, _position, null, Color.White, 0, Vector2.Zero, _scale, SpriteEffects.None, 1);
    }

    public void Move()
    {
        _position += _velocity * (float)Globals.ElapsedSeconds;
        //KeepInBounds();
    }

    public virtual void KeepInBounds()
    {
        _position = Vector2.Clamp(_position, Vector2.Zero, new Vector2(Config.WindowSize.X - _texture.Width * _scale, Config.WindowSize.Y - _texture.Height * _scale));
    }

    #region Collision


    public void HandleCollisionBird()
    {
        List<ICollidable> collisions = CheckCollisions();
        foreach (var obj in collisions)
        {
            ResolveCollision(obj);
        }

    }

    public bool HandleCollisionsNonPhys()
    {
        List<ICollidable> collisions = CheckCollisions();
        bool isBlocked = false;
        foreach (var obj in collisions)
        {
            if (obj is InvisibleBarrier && this is PlatformEnemy) isBlocked = true;
            else if ((IsTouchingLeft(obj) || IsTouchingRight(obj)) &&(!IsTouchingTop(obj) && obj is not Nonphysical)) isBlocked = true;

            ResolveCollision(obj);
        }

        _isOnGround = _lastGround != null && collisions.Contains(_lastGround) && IsTouchingTop(_lastGround);
        if (!_isOnGround && isBlocked) isBlocked = false;
        return isBlocked;
    }

    public void bounceOnEnemy(Nonphysical obj)
    {
        _lastGround = obj;
        _isOnEnemyBounce = true;
        Globals.CurrentLevel.toRemoveEnemy = obj;
    }

    public bool CheckPlayerHurt(ICollidable obj)
    {
        if (this is Actor && obj is Nonphysical) {

            if (IsTouchingTop(obj) && _velocity.Y > 0 )
            {
                bounceOnEnemy(obj as Nonphysical);
                return true;
            }

            else if (!((Actor)(this)).isHurt)
            {
                ((Actor)(this)).GetHit();

                Globals.CurrentLevel.UpdateHealth();
            }
            return true;
        }


        return false;
    }

    public void HandleCollisions()
    {
        List<ICollidable> collisions = CheckCollisions();
        foreach (var obj in collisions)
        {

            if (obj is Note && this is Actor)
            {
                ((Note)obj).SetTouched();
            }

            else
            {

                if (CheckPlayerHurt(obj)) continue;
                ResolveCollision(obj);
            }
        }
        _isOnGround = _lastGround != null && collisions.Contains(_lastGround) && IsTouchingTop(_lastGround);
    }

    public List<ICollidable> CheckCollisions()
    {
        List<ICollidable> collisions = new();
        if (this is Actor)
        {
            if (CollidesWith(Globals.CurrentLevel.End)) { 

                Globals.CurrentLevel.EndSequence();

                Globals.CurrentLevel.UpdateHighScore();
                // when we get another level
                if(!Globals.CurrentLevel.isGamePaused()) Globals.CurrentLevel.RotateState();
              
                //Globals.Game.Exit();
            }
        }

        if (this is PlatformEnemy)
        {
            foreach (InvisibleBarrier i in Globals.CurrentLevel.InvisibleBarriers)
            {
                {
                    if (CollidesWith(i))
                        collisions.Add(i);
                }
            }
        }

        if (this is not FlyingEnemy)
        {

            foreach (Tile t in Globals.CurrentLevel.Tiles)
            {
                if (t is Note) continue;

                if (CollidesWith(t))
                    collisions.Add(t);
            }
            foreach (Note i in Globals.CurrentLevel.Notes)
            {
                if (this is not Actor) continue;
                if (CollidesWith(i))
                    collisions.Add(i);
            }

            foreach (Nonphysical i in Globals.CurrentLevel.NonPhysicals)
            {
                if (i == this || (this is Nonphysical && i is FlyingEnemy))
                    continue;

                if (CollidesWith(i))
                    collisions.Add(i);
            }

        }

        else
        {
            foreach (Nonphysical n in Globals.CurrentLevel.NonPhysicals)
            {
                {
                    if (this == n) continue;

                    if (n is FlyingEnemy)
                    {
                        if (CollidesWith(n))
                            collisions.Add(n);
                    }
                }
            }

        }

        return collisions;
    }

    public bool CollidesWith(ICollidable obj)
    {
        if (obj is SemiSolidPlatform )
            return IsTouchingTop(obj) && !IsTouchingBottom(obj);

        return Bounds.CollidesWith(obj.Bounds);
    }

    public void ResolveCollision(ICollidable obj)
    {

        // Determine depth of intersection to know which side is being collided with
        Vector2 depth = CollisionHelper.GetIntersectionDepth(Bounds, obj.Bounds);

        // depth can be zero because a platform does not actually occupy an entire Tiled tile
        if (depth != Vector2.Zero)
        {
            float absDepthX = Math.Abs(depth.X);
            float absDepthY = Math.Abs(depth.Y);


            // Resolve the collision along the shallow axis.
            if (absDepthY < absDepthX)
            {
                // If we crossed the top of a tile, we are on the ground.
                if (_velocity.Y >= 0)
                {
                    if (obj is BouncyPlatform) _isOnBounce = true;
                    else if(IsTouchingTop(obj))
                    {
                        _lastGround = obj;
                    }

                }

                // Ignore SemiSolidPlatforms, unless we are on the ground.
                if (obj is not SemiSolidPlatform || _isOnGround)
                {
                    // Resolve the collision along the Y axis.
                    _position.Y += depth.Y;

                    // If we were falling OR we hit our head on the bottom of an object, set our velocity to zero.
                    // The state machine will recognize this and change the state accordingly.
                    if (_velocity.Y > 0 || IsTouchingBottom(obj)) _velocity.Y = 0;
                }
            }


            else if (obj is not SemiSolidPlatform) // Ignore SemiSolidPlatforms (don't care about X-axis collisions)
            {
                // Push interactables
                if (obj is Interactable)
                {
                    Push(obj as Interactable);

                    // get new depth
                    depth = CollisionHelper.GetIntersectionDepth(Bounds, obj.Bounds);
                }
                // Resolve the collision along the X axis.
                _position.X += depth.X;
                _velocity.X = 0;
            }
            
        }
        if (obj is TimedPlatform)
        {
            ((TimedPlatform)obj).SetTouched();
        }
        if (obj is MovingPlatform && IsTouchingTop(obj))
        {
            ((MovingPlatform)obj).SetTouched(true);
        }
    }

    public void Push(Interactable i)
    {
        Vector2 pos = i.Position;

        pos.X += _velocity.X * i.PushDrag * (float)Globals.ElapsedSeconds;
        i.Position = pos;
        i.HandleCollisions();
    }

    public bool IsTouchingTop(ICollidable obj)
    {
        return Bounds.IsTouchingTop(obj.Bounds);
    }

    public bool IsTouchingBottom(ICollidable obj)
    {
        return Bounds.IsTouchingBottom(obj.Bounds);
    }

    public bool IsTouchingLeft(ICollidable obj)
    {
        return Bounds.IsTouchingLeft(obj.Bounds);
    }

    public bool IsTouchingRight(ICollidable obj)
    {
        return Bounds.IsTouchingRight(obj.Bounds);
    }
    #endregion

    public override string ToString()
    {
        return $"{GetType()}: Position={_position} Bounds={Bounds} Velocity={_velocity}";
    }
}
