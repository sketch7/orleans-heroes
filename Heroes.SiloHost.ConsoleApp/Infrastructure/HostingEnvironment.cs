namespace Heroes.SiloHost.ConsoleApp.Infrastructure
{
	public class HostingEnvironment
	{
		public HostingEnvironment()
		{
			Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ProductionEnvironment;
			IsDocker = System.Environment.GetEnvironmentVariable("DOCKER") == "true";
		}

		public static string DevelopmentEnvironment { get; } = "Development";
		public static string ProductionEnvironment { get; } = "Production";

		public string Environment { get; }

		/// <summary>
		/// Determine whether its running as docker in development mode. Refrain from use as much as possible and instead use environments.
		/// </summary>
		public bool IsDocker { get; }

		public bool IsDockerDev => IsDocker && Environment == DevelopmentEnvironment;
		public bool IsDev => Environment == DevelopmentEnvironment;
		public bool IsProd => Environment == ProductionEnvironment;
	}
}