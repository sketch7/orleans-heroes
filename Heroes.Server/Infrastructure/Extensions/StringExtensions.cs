namespace Heroes.Server.Infrastructure;

public static class StringExtensions
{
	extension(string value)
	{
		/// <summary>Hypenate (kebab/dashed) value e.g. 'ChickenWings' → 'Chicken-wings'.</summary>
		public string ToHypenCase()
		{
			var chars = value.ToList();
			for (var i = 0; i < chars.Count - 1; i++)
			{
				if (char.IsWhiteSpace(chars[i]) || !char.IsUpper(chars[i + 1]))
					continue;
				chars[i + 1] = char.ToLower(chars[i + 1]);
				chars.Insert(i + 1, '-');
			}

			return new(chars.ToArray());
		}

		/// <summary>Determine whether string is null or empty.</summary>
		public bool IsNullOrEmpty() => string.IsNullOrEmpty(value);

		/// <summary>Determine whether string is null, empty, or matches the expected value.</summary>
		public bool IsNullOrEmptyOrEqual(string expectedValue)
			=> value.IsNullOrEmpty() || value == expectedValue;

		/// <summary>Invokes an action when value is null or empty.</summary>
		public void IfNullOrEmptyThen(Action action)
		{
			if (value.IsNullOrEmpty())
				action();
		}
	}
}
