namespace Heroes.Server.Infrastructure;

/// <summary>Application metadata interface.</summary>
public interface IAppInfo
{
	/// <summary>Get application name. e.g. '@odin/skeleton'.</summary>
	string Name { get; }

	/// <summary>Gets the application short name. e.g. 'skeleton'.</summary>
	string ShortName { get; }

	string ClusterId { get; set; }

	/// <summary>Get environment. e.g. 'Development'.</summary>
	string Environment { get; }

	/// <summary>Get git short commit hash. e.g. 'b603d6'</summary>
	string GitCommit { get; }

	/// <summary>Get application version. e.g. '1.1.0-staging'</summary>
	string Version { get; }

	/// <summary>Get whether the app is dockerized or not.</summary>
	bool IsDockerized { get; }

	/// <summary>Gets which service type is this app responsible of e.g. web, silo, etc...</summary>
	string ServiceType { get; set; }
}

public sealed class AppInfo : IAppInfo
{
	public string Name { get; set; } = string.Empty;
	public string ShortName { get; } = string.Empty;
	public string ClusterId { get; set; } = string.Empty;
	public string Environment { get; set; } = string.Empty;
	public string GitCommit { get; set; } = string.Empty;
	public string Version { get; set; } = string.Empty;
	public bool IsDockerized { get; set; }
	public string ServiceType { get; set; } = string.Empty;

	private static readonly Dictionary<string, string> EnvironmentMapping = new()
	{
		["Development"] = "dev",
		["Staging"] = "staging",
		["Production"] = "prod",
	};

	/// <summary>Resolve from <see cref="IConfiguration"/>.</summary>
	public AppInfo(IConfiguration config)
	{
		Name = config.GetValue("appName", "app") ?? "app";
		Version = config.GetValue("version", "local") ?? "local";
		GitCommit = config.GetValue("gitCommit", "-") ?? "-";
		var rawEnv = config.GetValue<string>("ASPNETCORE_ENVIRONMENT");
		IsDockerized = config.GetValue<bool>("DOCKER");
		ServiceType = config.GetValue("serviceType", "dotnet") ?? "dotnet";
		ShortName = Name.Split('/').Last();

		if (string.IsNullOrEmpty(rawEnv))
			throw new InvalidOperationException("Environment is not set. Please specify the environment via 'ASPNETCORE_ENVIRONMENT'");

		ClusterId = $"{Name}-{Version}";
		Environment = MapEnvironmentName(rawEnv);
	}

	/// <summary>Maps a full environment name to its short alias, or returns the original if not mapped.</summary>
	public static string MapEnvironmentName(string environment)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(environment);

		return EnvironmentMapping.TryGetValue(environment, out var env) ? env : environment;
	}
}
