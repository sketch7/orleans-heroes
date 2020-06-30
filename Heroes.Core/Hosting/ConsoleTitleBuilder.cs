using System;
using System.Runtime.InteropServices;

namespace Heroes.Core.Hosting
{
	/// <summary>
	/// Class to be able to set console title easier.
	/// </summary>
	public static class ConsoleTitleBuilder
	{
		/// <summary>
		/// Gets whether Title text is available.
		/// </summary>
		public static bool IsAvailable()
			=> Environment.UserInteractive && !RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		/// <summary>
		/// Set console title.
		/// </summary>
		/// <param name="title"></param>
		public static void Set(string title)
		{
			if (!IsAvailable())
				return;

			Console.Title = title;
		}

		/// <summary>
		/// Append additional text to title.
		/// </summary>
		/// <param name="title"></param>
		public static void Append(string title)
		{
			if (!IsAvailable())
				return;

			Console.Title += $" {title}";
		}

		/// <summary>
		/// Append additional text to title (lazy).
		/// </summary>
		/// <param name="func"></param>
		public static void Append(Func<string> func)
		{
			if (!IsAvailable())
				return;

			Console.Title += $" {func()}";
		}
	}
}
