namespace Util.Random
{
    public class RandomHelper
    {
        private readonly System.Random _random;

        public RandomHelper()
        {
            _random = new System.Random();
        }

        public byte[] GetBytes(int length)
        {
            if (length == 0)
                return new byte[0];

            var randomData = new byte[length];

            _random.NextBytes(randomData);

            return randomData;
        }

        public int GetInt(int min, int max)
            => max < min ? _random.Next(max, min) : _random.Next(min, max);
    }
}