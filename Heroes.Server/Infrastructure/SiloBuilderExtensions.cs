using Orleans.Configuration;
using System.Net;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace Heroes.Server.Infrastructure;

public enum StorageProviderType
{
	Memory,
	Redis
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class AppSiloOptions
{
	private string DebuggerDisplay => $"GatewayPort: '{GatewayPort}', SiloPort: '{SiloPort}'";

	public int GatewayPort { get; set; } = 30000;
	public int SiloPort { get; set; } = 11111;
	public StorageProviderType? StorageProviderType { get; set; }
}

public sealed record AppSiloBuilderContext
{
	public required HostBuilderContext HostBuilderContext { get; init; }
	public required IAppInfo AppInfo { get; init; }
	public required AppSiloOptions SiloOptions { get; init; }
}

public static class SiloBuilderExtensions
{
	private static StorageProviderType _defaultProviderType;

	extension(ISiloBuilder siloHost)
	{
		public ISiloBuilder UseAppConfiguration(AppSiloBuilderContext context)
		{
			_defaultProviderType = context.SiloOptions.StorageProviderType ?? StorageProviderType.Memory;

			var appInfo = context.AppInfo;
			siloHost
				.AddMemoryGrainStorage(OrleansConstants.GrainMemoryStorage)
				.UseStorage(OrleansConstants.GrainPersistenceStorage, context.AppInfo)
				.UseStorage(OrleansConstants.PubSubStore, context.AppInfo)
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = appInfo.ClusterId;
					options.ServiceId = appInfo.Name;
				});

			if (context.HostBuilderContext.HostingEnvironment.IsDevelopment())
			{
				siloHost.UseDevelopment(context);
				siloHost.UseDevelopmentClustering(context);
			}
			// if (appInfo.IsDockerized)
			// 	siloHost.UseDockerSwarm(context);
			else
			{
				// Production clustering would go here
				siloHost.UseDevelopmentClustering(context);
			}

			return siloHost;
		}

		private ISiloBuilder UseDevelopment(AppSiloBuilderContext context)
		{
			siloHost
				.ConfigureServices(services =>
				{
					//services.Configure<GrainCollectionOptions>(options => { options.CollectionAge = TimeSpan.FromMinutes(1.5); });
				});
			//.Configure<ClusterMembershipOptions>(options => options.ExpectedClusterSize = 1);

			return siloHost;
		}

		private ISiloBuilder UseDevelopmentClustering(AppSiloBuilderContext context)
		{
			var siloAddress = IPAddress.Loopback;
			var siloPort = context.SiloOptions.SiloPort;
			var gatewayPort = context.SiloOptions.GatewayPort;

			return siloHost
					.UseLocalhostClustering(siloPort: siloPort, gatewayPort: gatewayPort)
				//.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
				//.ConfigureEndpoints(siloAddress, siloPort, gatewayPort) //, listenOnAnyHostAddress: true)
				;
		}

		public ISiloBuilder UseStorage(string storeProviderName, IAppInfo appInfo, StorageProviderType? storageProvider = null, string? storeName = null)
		{
			storeName = string.IsNullOrEmpty(storeName) ? storeProviderName : storeName;
			storageProvider ??= _defaultProviderType;

			switch (storageProvider)
			{
				case StorageProviderType.Memory:
					siloHost.AddMemoryGrainStorage(storeProviderName);
					break;
				case StorageProviderType.Redis:
					throw new NotSupportedException("Redis storage is not configured in this build. Add a Redis persistence package compatible with the current Orleans version.");
				// siloBuilder
				// 	.AddRedisGrainStorage(storeProviderName)
				// 	.Build(builder =>
				// 		builder.Configure(ConfigureRedisOptions(storeName, appInfo))
				// 	);
				// break;
				default:
					throw new ArgumentOutOfRangeException(nameof(storageProvider), $"Storage provider '{storageProvider}' is not supported.");
			}

			return siloHost;
		}
	}

	//private static Action<RedisStorageOptions> ConfigureRedisOptions(
	//	string tableType,
	//	IAppInfo appInfo
	//)
	//{
	//	// todo: make configurable
	//	return config =>
	//	{
	//		config.Servers = new[] { "localhost" };
	//		config.ClientName = appInfo.ClusterId;
	//		config.KeyPrefix = $"{appInfo.ShortName}-{tableType}";
	//		config.HumanReadableSerialization = true;
	//	};
	//}
}