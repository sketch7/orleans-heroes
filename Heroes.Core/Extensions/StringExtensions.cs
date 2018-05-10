using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Heroes.Core
{
	public static class StringExtensions
	{
		/// <summary>
		/// Hypenate (kebab/dashed) value e.g. 'ChickenWings' => 'Chicken-wings'.
		/// </summary>
		/// <param name="value">Value to hypenate.</param>
		/// <returns></returns>
		public static string ToHypenCase(this string value)
		{
			var chars = value.ToList();
			for (int i = 0; i < chars.Count - 1; i++)
			{
				if (char.IsWhiteSpace(chars[i]) || !char.IsUpper(chars[i + 1]))
					continue;
				chars[i + 1] = char.ToLower(chars[i + 1]);
				chars.Insert(i + 1, '-');
			}
			return new string(chars.ToArray());
		}

		/// <summary>
		/// Determine whether string is null or empty.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>Returns true when null or empty.</returns>
		public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

		/// <summary>
		/// Determine whether string is null or empty or matches the expected value.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="expectedValue">Value to check.</param>
		/// <returns></returns>
		public static bool IsNullOrEmptyOrEqual(this string value, string expectedValue)
			=> value.IsNullOrEmpty() || value == expectedValue;

		/// <summary>
		/// Invokes an action when value is null or empty.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="action">Action to invoke when null/empty.</param>
		public static void IfNullOrEmptyThen(this string value, Action action)
		{
			if (value.IsNullOrEmpty())
				action();
		}

		/// <summary>
		/// Invokes an action when value is not null or empty.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="action">Action to invoke when not null/empty.</param>
		public static void IfNotNullOrEmptyThen(this string value, Action action)
		{
			if (!value.IsNullOrEmpty())
				action();
		}

		/// <summary>
		/// Invokes an action and return a new value, when value is null/empty, else return original value.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="func">Action to invoke when null/empty which returns a new value.</param>
		/// <returns>Returns new value from func when null/empty or original value.</returns>
		public static string IfNullOrEmptyReturn(this string value, Func<string> func) => value.IsNullOrEmpty() ? func() : value;

		/// <summary>
		/// Returns defaultValue when null/empty, else return original value.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="defaultValue">Value to return when null/empty.</param>
		public static string IfNullOrEmptyReturn(this string value, string defaultValue) => value.IsNullOrEmpty() ? defaultValue : value;

		/// <summary>
		/// Trims all leading occurrences of a string from the string.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="trimString">Value to trim.</param>
		/// <returns></returns>
		public static string TrimStart(this string value, string trimString)
		{
			var result = value;
			while (result.StartsWith(trimString))
			{
				result = result.Substring(trimString.Length);
			}

			return result;
		}

		/// <summary>
		/// Trims all trailing occurrences of a string from the string.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="trimValue">Value to trim.</param>
		/// <returns></returns>
		public static string TrimEnd(this string value, string trimValue)
		{
			var result = value;
			while (result.EndsWith(trimValue))
			{
				result = result.Substring(0, result.Length - trimValue.Length);
			}

			return result;
		}
	}
}
