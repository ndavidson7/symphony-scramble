// Adapted from https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie004-SpritesheetAnimation

using Microsoft.Xna.Framework.Content;

namespace SymphonyScramble;

public static class Globals
{
    public static double ElapsedSeconds { get; private set; }
    public static double TotalSeconds { get; private set; }
    public static KeyboardState KeyboardState { get; private set; }
    public static Game Game { get; internal set; }
    public static ContentManager Content { get; internal set; }
    public static SpriteBatch SpriteBatch { get; internal set; }
    public static GraphicsDevice GraphicsDevice { get; internal set; }
    public static Level CurrentLevel { get; internal set; }
    public static Texture2D DebugTexture
    {
        get
        {
            var debugTexture = new Texture2D(GraphicsDevice, 1, 1);
            debugTexture.SetData([Color.DarkSlateGray]);
            return debugTexture;
        }
    }

    public static void Update(GameTime gt, KeyboardState kb)
    {
        ElapsedSeconds = gt.ElapsedGameTime.TotalSeconds;
        TotalSeconds += ElapsedSeconds;
        KeyboardState = kb;
    }

    public static void ResetTime()
    {
        TotalSeconds = 0;
    }
}