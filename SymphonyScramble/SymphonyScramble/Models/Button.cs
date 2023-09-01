using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymphonyScramble;

/*
 * REFERENCES
 * Title: MonoGame_Tutorials/Tutorial013
 * Author: Niall Lewin (Oyyou on Github)
 * Date: 4/24/2023
 * URL: https://github.com/Oyyou/MonoGame_Tutorials/tree/master/MonoGame_Tutorials/Tutorial013
 */
public class Button
{
    #region Fields
    private MouseState _mouseState;

    private MouseState _prevMouseState;

    private Texture2D _texture;

    private SpriteFont _font;

    private bool _isHovering;

    #endregion

    #region Properties
    public event EventHandler Click;

    public bool Clicked { get; private set; }

    public Vector2 Position { get; set; }

    public Rectangle Rectangle
    {
        get
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
        }
    }

    public string Text { get; set; }
    #endregion

    #region Methods
    public Button (Texture2D texture, SpriteFont font)
    {
        _texture = texture;
        _font = font;
    }

    public void Draw()
    {
        var color = Color.White;

        if (_isHovering)
            color = Color.Gray;

        Globals.SpriteBatch.Draw(_texture, Rectangle, color);

        if (!string.IsNullOrEmpty(Text))
        {
            var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
            var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

            Globals.SpriteBatch.DrawString(_font, Text, new Vector2(x, y), Color.Black); // draws text on box
        }
    }

    public void Update ()
    {
        _prevMouseState = _mouseState;
        _mouseState = Mouse.GetState();

        var mouseRect = new Rectangle(_mouseState.X, _mouseState.Y, 1, 1);

        _isHovering = false;

        if (mouseRect.Intersects(Rectangle))
        {
            _isHovering = true;

            if (_mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
            {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
    #endregion
}

