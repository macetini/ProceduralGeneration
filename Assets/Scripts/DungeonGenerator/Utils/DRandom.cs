using System;

namespace Assets.Scripts.DungeonGenerator.Utils
{
    public class DRandom
    {
        public int Seed { get; set; } = 0;
        public Random random;

        public bool init = false;
        public void Init(int _seed = 0)
        {
            if (!init)
            {                
                init = true;
                Seed = _seed;
                random = new Random(Seed);
            }
        }

        public float Value()
        {
            Init(Seed);
            return (float)random.NextDouble();
        }

        /// <summary>
        /// returns a random int between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int RangeInt(int min, int max)
        {
            return (int)((max - min + 1) * Value()) + min;
        }
    }
}