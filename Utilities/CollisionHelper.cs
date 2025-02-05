// Adapted from https://textbooks.cs.ksu.edu/cis580/04-collisions/

using System;

namespace SymphonyScramble;

/// <summary>
/// A class containing collision detection methods
/// </summary>
public static class CollisionHelper
{
    /// <summary>
    /// Detects a collision between two points
    /// </summary>
    /// <param name="p1">the first point</param>
    /// <param name="p2">the second point</param>
    /// <returns>true when colliding, false otherwise</returns>
    public static bool Collides(BoundingPoint p1, BoundingPoint p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    /// <summary>
    /// Detects a collision between two circles
    /// </summary>
    /// <param name="c1">the first circle</param>
    /// <param name="c2">the second circle</param>
    /// <returns>true for a collision, false otherwise</returns>
    public static bool Collides(BoundingCircle c1, BoundingCircle c2)
    {
        return Math.Pow(c1.Radius + c2.Radius, 2) >= Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2);
    }

    /// <summary>
    /// Detects a collision between two rectangles
    /// </summary>
    /// <param name="r1">The first rectangle</param>
    /// <param name="r2">The second rectangle</param>
    /// <returns>true on collision, false otherwise</returns>
    public static bool Collides(BoundingRectangle r1, BoundingRectangle r2)
    {
        return !(r1.X + r1.Width < r2.X    // r1 is to the left of r2
                || r1.X > r2.X + r2.Width     // r1 is to the right of r2
                || r1.Y + r1.Height < r2.Y    // r1 is above r2 
                || r1.Y > r2.Y + r2.Height); // r1 is below r2
    }

    /// <summary>
    /// Detects a collision between a circle and point
    /// </summary>
    /// <param name="c">the BoundingCircle</param>
    /// <param name="p">the BoundingPoint</param>
    /// <returns>true on collision, false otherwise</returns>
    public static bool Collides(BoundingCircle c, BoundingPoint p)
    {
        return Math.Pow(c.Radius, 2) >= Math.Pow(c.X - p.X, 2) + Math.Pow(c.Y - p.Y, 2);
    }

    /// <summary>
    /// Detects a collision between a rectangle and point
    /// </summary>
    /// <param name="r">The BoundingRectangle</param>
    /// <param name="p">The BoundingPoint</param>
    /// <returns>true on collision, false otherwise</returns>
    public static bool Collides(BoundingRectangle r, BoundingPoint p)
    {
        return p.X >= r.X && p.X <= r.X + r.Width && p.Y >= r.Y && p.Y <= r.Y + r.Height;
    }

    /// <summary>
    /// Determines if there is a collision between a rectangle and circle
    /// </summary>
    /// <param name="r">The BoundingRectangle</param>
    /// <param name="c">The BoundingCircle</param>
    /// <returns>true for collision, false otherwise</returns>
    public static bool Collides(BoundingRectangle r, BoundingCircle c)
    {
        BoundingPoint p;
        p.X = MathHelper.Clamp(c.X, r.X, r.X + r.Width);
        p.Y = MathHelper.Clamp(c.Y, r.Y, r.Y + r.Height);
        return Collides(c, p);
    }

    /// <summary>
    /// Calculates the signed depth of intersection between two rectangles.
    /// Originally authored for MonoGame Platformer2D Sample Game (NOT MY WORK, SEE LINK FOR SOURCE).
    /// Adapted to fit the more precise BoundingRectangle defined in this file.
    /// </summary>
    /// <returns>
    /// The amount of overlap between two intersecting rectangles. These
    /// depth values can be negative depending on which wides the rectangles
    /// intersect. This allows callers to determine the correct direction
    /// to push objects in order to resolve collisions.
    /// If the rectangles are not intersecting, Vector2.Zero is returned.
    /// </returns>
    /// <see href="https://github.com/MonoGame/MonoGame.Samples/blob/b277067deda3dd57fe67b4927f6df305de12cd06/Platformer2D/Platformer2D.Core/Game/RectangleExtensions.cs#L30"/>
    public static Vector2 GetIntersectionDepth(BoundingRectangle rectA, BoundingRectangle rectB)
    {
        // Calculate half sizes.
        float halfWidthA = rectA.Width / 2.0f;
        float halfHeightA = rectA.Height / 2.0f;
        float halfWidthB = rectB.Width / 2.0f;
        float halfHeightB = rectB.Height / 2.0f;

        // Calculate centers.
        Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
        Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

        // Calculate current and minimum-non-intersecting distances between centers.
        float distanceX = centerA.X - centerB.X;
        float distanceY = centerA.Y - centerB.Y;
        float minDistanceX = halfWidthA + halfWidthB;
        float minDistanceY = halfHeightA + halfHeightB;

        // If we are not intersecting at all, return (0, 0).
        if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            return Vector2.Zero;

        // Calculate and return intersection depths.
        float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        return new Vector2(depthX, depthY);
    }
}

/// <summary>
/// A struct representing a bounding circle for determining collisions
/// </summary>
public struct BoundingCircle
{
    public float X;
    public float Y;
    public float Radius;

    /// <summary>
    /// Constructs a BoundingCircle with the provided coordinates
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <param name="radius">The radius</param>
    public BoundingCircle(float x, float y, float radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    /// <summary>
    /// Determines if this BoundingCircle collides with a BoundingPoint
    /// </summary>
    /// <param name="p">the BoundingPoint</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingPoint p)
    {
        return CollisionHelper.Collides(this, p);
    }

    /// <summary>
    /// Determines if this BoundingCircle collides with another BoundingCircle
    /// </summary>
    /// <param name="c">the other BoundingCircle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingCircle c)
    {
        return CollisionHelper.Collides(this, c);
    }

    /// <summary>
    /// Determines if this BoundingCircle collides with a BoundingRectangle
    /// </summary>
    /// <param name="r">the BoundingRectangle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingRectangle r)
    {
        return CollisionHelper.Collides(r, this);
    }
}

/// <summary>
/// A struct representing a bounding rectangle for determining collisions
/// </summary>
public struct BoundingRectangle
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left => X;
    public float Top => Y;
    public float Right => X + Width;
    public float Bottom => Y + Height;

    /// <summary>
    /// Constructs a BoundingRectangle with the provided coordinates
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <param name="width">The width</param>
    /// <param name="height">The height</param>
    public BoundingRectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Determines if this BoundingRectangle collides with BoundingPoint
    /// </summary>
    /// <param name="p">the BoundingPoint</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingPoint p)
    {
        return CollisionHelper.Collides(this, p);
    }

    /// <summary>
    /// Determines if this BoundingRectangle collides with a BoundingCircle
    /// </summary>
    /// <param name="c">the BoundingCircle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingCircle c)
    {
        return CollisionHelper.Collides(this, c);
    }

    /// <summary>
    /// Determines if this BoundingRectangle collides with another BoundingRectangle
    /// </summary>
    /// <param name="r">the BoundingRectangle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingRectangle r)
    {
        return CollisionHelper.Collides(this, r);
    }

    public bool IsTouchingTop(BoundingRectangle r)
    {
        return Bottom >= r.Top &&
            Top < r.Top &&
            Right > r.Left &&
            Left < r.Right;
    }

    public bool IsTouchingBottom(BoundingRectangle r)
    {
        return Top <= r.Bottom &&
            Bottom > r.Bottom &&
            Right > r.Left &&
            Left < r.Right;
    }

    public bool IsTouchingLeft(BoundingRectangle r)
    {
        return Right >= r.Left &&
            Left < r.Left &&
            Bottom > r.Top &&
            Top < r.Bottom;
    }

    public bool IsTouchingRight(BoundingRectangle r)
    {
        return Left <= r.Right &&
            Right > r.Right &&
            Bottom > r.Top &&
            Top < r.Bottom;
    }
}

/// <summary>
/// A struct representing a bounding point for determining collisions
/// </summary>
public struct BoundingPoint
{
    public float X;
    public float Y;

    /// <summary>
    /// Constructs a BoundingPoint with the provided coordinates
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    public BoundingPoint(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Determines if this BoundingPoint collides with another BoundingPoint
    /// </summary>
    /// <param name="p">the other BoundingPoint</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingPoint p)
    {
        return CollisionHelper.Collides(p, this);
    }

    /// <summary>
    /// Determines if this BoundingPoint collides with a BoundingCircle
    /// </summary>
    /// <param name="c">the BoundingCircle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingCircle c)
    {
        return CollisionHelper.Collides(c, this);
    }

    /// <summary>
    /// Determines if this BoundingPoint collides with a BoundingRectangle
    /// </summary>
    /// <param name="r">the BoundingRectangle</param>
    /// <returns>true on collision, false otherwise</returns>
    public bool CollidesWith(BoundingRectangle r)
    {
        return CollisionHelper.Collides(r, this);
    }
}