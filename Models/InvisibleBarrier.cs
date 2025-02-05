using System;
namespace SymphonyScramble
{
	public class InvisibleBarrier: SolidPlatform
	{

        private const float DEFAULT_SCALE = 1f;
        private const int UNSCALED_WIDTH = 8;
        private const int UNSCALED_HEIGHT = 8;

        public  bool _isPlatformBarrier ;


        public InvisibleBarrier(Vector2 position, float scale = DEFAULT_SCALE, bool isPlatformBarrier = false) : base(position, scale)
        {
            _isPlatformBarrier = isPlatformBarrier;
        }

       
    }
}

