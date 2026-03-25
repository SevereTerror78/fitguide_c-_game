using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class CouponGenerator
    {
        private static readonly Random rnd = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Generates a random alphanumeric coupon code of the requested length.
        /// </summary>
        /// <param name="length">Desired length of the generated code.</param>
        /// <returns>Random coupon string.</returns>
        public static string Generate(int length = 12)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
