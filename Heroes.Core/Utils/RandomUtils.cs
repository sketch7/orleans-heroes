using System;
using System.Text;

namespace Heroes.Core.Utils
{
	public static class RandomUtils
	{
		private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		private static readonly Random random = new Random();

		/// <summary>
		/// Generates a random string consisting of uppercase and lowercase alpha characters e.g. 'chgKr'.
		/// </summary>
		/// <param name="minLength">Minimum generated string length defaults to 3.</param>
		/// <param name="maxLength">Maximum generated string length defaults to 10.</param>
		/// <returns>String</returns>
		public static string GenerateString(int minLength = 3, int maxLength = 10)
		{
			var stringBuilder = new StringBuilder();
			var stringLength = GenerateNumber(minLength, maxLength);

			for (var i = 0; i < stringLength; i++)
			{
				var num = GenerateNumber(0, Chars.Length - 1);
				stringBuilder.Append(Chars[num]);
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Generate a random number e.g. "21".
		/// </summary>
		/// <param name="min">Minimum value allowed (defaults: 1)</param>
		/// <param name="max">Maximum value allowed (defaults: 10)</param>
		/// <returns></returns>
		public static int GenerateNumber(int min = 1, int max = 10) => random.Next(min, max + 1);
	}
}