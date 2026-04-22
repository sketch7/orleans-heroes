namespace Heroes.Server.Infrastructure.Hosting;

/// <summary>Helpers for setting the console window title.</summary>
public static class ConsoleTitleBuilder
{
	/// <summary>Gets whether console title changes are supported (Windows/interactive only).</summary>
	public static bool IsAvailable()
		=> Environment.UserInteractive && OperatingSystem.IsWindows();

	/// <summary>Set console title.</summary>
	public static void Set(string title)
	{
		if (!IsAvailable())
			return;

		Console.Title = title;
	}

	/// <summary>Append additional text to title.</summary>
	public static void Append(string title)
	{
		if (!Environment.UserInteractive || !OperatingSystem.IsWindows())
			return;

		Console.Title += $" {title}";
	}
}
