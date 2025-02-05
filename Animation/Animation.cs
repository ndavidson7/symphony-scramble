// Adapted from https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie004-SpritesheetAnimation

using System.Collections.Generic;

namespace SymphonyScramble;

/// <summary>
/// An Animation is an ordered collection of frames on a spritesheet that when drawn
/// back-to-back in a short duration gives the appearance of a moving character.
/// </summary>
public class Animation
{
    #region Fields
    private readonly Texture2D _texture;
    private readonly List<Rectangle> _sourceRectangles = new();
    private float _scale;
    private readonly int _frames;
    private int _frame;
    private readonly float _frameTime;
    private float _frameTimeLeft;
    private readonly bool _looping;
    private bool _active = true;
    #endregion

    #region Properties
    public bool IsFinished => !_active;

    #endregion

    /// <summary>
    /// Create an Animation using a spritesheet that has each animation on an entire row or column,
    /// the same number of frames in each animation, and identically-sized frames.
    /// </summary>
    /// <example>3 frames wide by 4 frames high, 3 animations (one per row), 4 frames per animation, each frame is same size</example>
    /// <param name="texture">The spritesheet loaded from Content</param>
    /// <param name="framesX">Max number of frames per row</param>
    /// <param name="framesY">Max number of frames per column</param>
    /// <param name="frameTime">Duration of each individual frame (in seconds?)</param>
    /// <param name="index">Index of animation (1-based)</param>
    /// <param name="byRow">Whether the animations are per row (false if per column)</param>
    /// <param name="scale">Scaling factor for sprite drawing</param>
    public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int index = 1, bool byRow = true, float scale = 1f, bool looping = true)
    {
        _texture = texture;
        _scale = scale;
        _frameTime = frameTime;
        _frameTimeLeft = _frameTime;
        _frames = byRow ? framesX : framesY;
        _looping = looping;
        var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;

        for (int i = 0; i < _frames; i++)
        {
            if (byRow) _sourceRectangles.Add(new Rectangle(i * frameWidth, (index - 1) * frameHeight, frameWidth, frameHeight));
            else _sourceRectangles.Add(new Rectangle((index - 1) * frameWidth, i * frameHeight, frameWidth, frameHeight));
        }
    }

    /// <summary>
    /// Create an Animation using a spritesheet that has each animation on part of a row or column,
    /// a variable number of frames in each animation, and identically-sized frames.
    /// </summary>
    /// <param name="texture">The spritesheet loaded from Content</param>
    /// <param name="framesX">Max number of frames per row</param>
    /// <param name="framesY">Max number of frames per column</param>
    /// <param name="frameTime">Duration of each individual frame (in seconds?)</param>
    /// <param name="index">Index of animation (1-based)</param>
    /// <param name="startFrame">Index within row/column of first frame (1-based)</param>
    /// <param name="endFrame">Index within row/column of last frame (1-based)</param>
    /// <param name="byRow">Whether the animations are per row (false if per column)</param>
    /// <param name="scale">Scaling factor for sprite drawing</param>
    public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int index, int startFrame, int endFrame, bool byRow = true, float scale = 1f, bool looping = true)
    {
        _texture = texture;
        _scale = scale;
        _frameTime = frameTime;
        _frameTimeLeft = _frameTime;
        _frames = 1 + endFrame - startFrame;
        _looping = looping;
        var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;

        for (int i = startFrame - 1; i < endFrame; i++)
        {
            if (byRow) _sourceRectangles.Add(new Rectangle(i * frameWidth, (index - 1) * frameHeight, frameWidth, frameHeight));
            else _sourceRectangles.Add(new Rectangle((index - 1) * frameWidth, i * frameHeight, frameWidth, frameHeight));
        }
    }

    /// <summary>
    /// Stops the Animation from playing (will continue drawing the last active frame, however)
    /// </summary>
    public void Stop()
    {
        _active = false;
    }

    /// <summary>
    /// Starts the Animation, i.e., begins iterating through frames
    /// </summary>
    public void Start()
    {
        _active = true;
    }

    /// <summary>
    /// Resets the Animation to the first frame
    /// </summary>
    public void Reset()
    {
        _frame = 0;
        _frameTimeLeft = _frameTime;
        _active = true;
    }

    /// <summary>
    /// Changes the scale at which the Animation's frames are drawn
    /// </summary>
    /// <param name="scale">New scaling factor</param>
    public void Rescale(float scale)
    {
        _scale = scale;
    }

    /// <summary>
    /// Increments the current frame when necessary
    /// </summary>
    public void Update()
    {
       
        if (!_active) return;

        _frameTimeLeft -= (float)Globals.ElapsedSeconds;

        if (_frameTimeLeft <= 0)
        {
            _frameTimeLeft += _frameTime;
            if (_looping)
            {
                _frame = (_frame + 1) % _frames;
            }
            else if (_active)
            {
                if (_frame < _frames - 1)
                {
                    _frame += 1;
                }
                else
                {
                    _active = false;
                }
            }


        }
    }

    /// <summary>
    /// Draws the current frame
    /// </summary>
    /// <param name="position">Position in MonoGame coordinates to draw to</param>
    /// <param name="flip">Whether the drawing should be flipped horizontally</param>
    public void Draw(Vector2 position, bool flip, bool isHurt = false)
    {
        if (isHurt)  Globals.SpriteBatch.Draw(_texture, position, _sourceRectangles[_frame], Color.Red, 0, Vector2.Zero, _scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        else Globals.SpriteBatch.Draw(_texture, position, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, _scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
    }
}