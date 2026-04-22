using System.Text;

namespace Heroes.Server.Infrastructure.Utils;

public static class RandomUtils
{
	private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	private static readonly Random _random = new();

	/// <summary>Generates a random alpha string e.g. 'chgKr'.</summary>
	/// <param name="minLength">Minimum length (default: 3).</param>
	/// <param name="maxLength">Maximum length (default: 10).</param>
	public static string GenerateString(int minLength = 3, int maxLength = 10)
	{
		var sb = new StringBuilder();
		var length = GenerateNumber(minLength, maxLength);

		for (var i = 0; i < length; i++)
			sb.Append(Chars[GenerateNumber(0, Chars.Length - 1)]);

		return sb.ToString();
	}

	/// <summary>Generate a random integer in the given range (inclusive).</summary>
	/// <param name="min">Minimum value (default: 1).</param>
	/// <param name="max">Maximum value (default: 10).</param>
	public static int GenerateNumber(int min = 1, int max = 10) => _random.Next(min, max + 1);
}
