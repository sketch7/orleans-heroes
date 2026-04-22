namespace Heroes.Server.Infrastructure;

public static class StringExtensions
{
	/// <summary>Hypenate (kebab/dashed) value e.g. 'ChickenWings' → 'Chicken-wings'.</summary>
	public static string ToHypenCase(this string value)
	{
		var chars = value.ToList();
		for (var i = 0; i < chars.Count - 1; i++)
		{
			if (char.IsWhiteSpace(chars[i]) || !char.IsUpper(chars[i + 1]))
				continue;
			chars[i + 1] = char.ToLower(chars[i + 1]);
			chars.Insert(i + 1, '-');
		}

		return new string(chars.ToArray());
	}

	/// <summary>Determine whether string is null or empty.</summary>
	public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

	/// <summary>Determine whether string is null, empty, or matches the expected value.</summary>
	public static bool IsNullOrEmptyOrEqual(this string value, string expectedValue)
		=> value.IsNullOrEmpty() || value == expectedValue;

	/// <summary>Invokes an action when value is null or empty.</summary>
	public static void IfNullOrEmptyThen(this string value, Action action)
	{
		if (value.IsNullOrEmpty())
			action();
	}
}
