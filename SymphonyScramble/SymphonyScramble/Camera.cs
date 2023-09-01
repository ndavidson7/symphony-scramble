using System;
namespace SymphonyScramble
{
    // using tutorial from Oyyou
    // https://www.youtube.com/watch?v=ceBCDKU_mNw
    public class Camera
	{
		public Camera()
		{
			
		}
        public Matrix Transform { get; private set; }

        public void Follow(Sprite target)
        {
            var _scale = 4.0f;
            var position = Matrix.CreateTranslation(-target.Position.X - (target.Bounds.Width / 2),
              -target.Position.Y - (target.Bounds.Height / 2), 0);

            var offset = Matrix.CreateTranslation(Config.WindowSize.X / 2, Config.WindowSize.Y / 2, 0);

            Transform = position * Matrix.CreateScale(_scale, _scale, 1f) * offset / 2;
        }
	}
}

