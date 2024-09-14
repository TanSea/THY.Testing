namespace THY.Testing.Common
{
    public class RandomNumber
    {
        private static readonly long _tick = DateTime.Now.Ticks;
        private static readonly Random _random = new((int)(_tick & 0xffffffffL) | (int)(_tick >> 32));

        public static int GenerateRandomScore(int min = -1000, int max = 1000)
        {
            return _random.Next(min, max);
        }
    }
}
