namespace SymphonyScramble;

public static class TextureHelper
{
    /// <summary>
    /// Return a Texture2D flipped vertically, horizontally, or both.
    /// Source: https://stackoverflow.com/a/22521184
    /// </summary>
    /// <param name="input">Texture2D to be flipped</param>
    /// <param name="vertical">Whether to flip the input upside down</param>
    /// <param name="horizontal">Whether to flip the input sideways</param>
    /// <returns>New Texture2D flipped</returns>
    /// <see href="https://stackoverflow.com/a/22521184">Examples</see>
    public static Texture2D Flipped(Texture2D input, bool vertical, bool horizontal)
    {
        Texture2D flipped = new Texture2D(input.GraphicsDevice, input.Width, input.Height);
        Color[] data = new Color[input.Width * input.Height];
        Color[] flipped_data = new Color[data.Length];

        input.GetData<Color>(data);

        for (int x = 0; x < input.Width; x++)
        {
            for (int y = 0; y < input.Height; y++)
            {
                int index = 0;
                if (horizontal && vertical)
                    index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                else if (horizontal && !vertical)
                    index = input.Width - 1 - x + y * input.Width;
                else if (!horizontal && vertical)
                    index = x + (input.Height - 1 - y) * input.Width;
                else if (!horizontal && !vertical)
                    index = x + y * input.Width;

                flipped_data[x + y * input.Width] = data[index];
            }
        }

        flipped.SetData<Color>(flipped_data);

        return flipped;
    }
}
