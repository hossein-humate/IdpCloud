using System;
using System.Text;

namespace IdpCloud.Common
{
    /// <inheritdoc/>
    public class RandomPasswordGenerator : IRandomPasswordGenerator
    {
        /// <inheritdoc/>
        public string RandomPassword()
        {
            var builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        /// <summary>
        /// Generate a random number between two numbers    
        /// </summary>
        /// <param name="min">Min number</param>
        /// <param name="max">Max Number</param>
        /// <returns>Random number</returns>

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        /// <summary>
        /// Generate a random string with a given size and case.   
        /// If second parameter is true, the return string is lowercase
        /// </summary>
        /// <param name="size">Required size of string</param>
        /// <param name="lowerCase">Required string expected is lower case or not</param>
        /// <returns>Returns random string</returns>
        private string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }
            return builder.ToString();
        }
    }
}
