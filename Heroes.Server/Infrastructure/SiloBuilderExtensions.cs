using Heroes.Contracts.Grains;
using Heroes.Core;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace Heroes.Server.Infrastructure
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class AppSiloOptions
	{
		private string DebuggerDisplay => $"GatewayPort: '{GatewayPort}', SiloPort: '{SiloPort}'";

		public int GatewayPort { get; set; } = 30000;
		public int SiloPort { get; set; } = 11111;
	}

	public class AppSiloBuilderContext
	{
		public HostBuilderContext HostBuilderContext { get; set; }
		public IAppInfo AppInfo { get; set; }
		public AppSiloOptions SiloOptions { get; set; }
	}

	public static class SiloBuilderExtensions
	{

		public static ISiloBuilder UseAppConfiguration(this ISiloBuilder siloHost, AppSiloBuilderContext context)
		{
			var appInfo = context.AppInfo;
			siloHost
				.AddMemoryGrainStorage(OrleansConstants.GrainMemoryStorage)
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = appInfo.ClusterId;
					options.ServiceId = appInfo.Name;
				});

			if (context.HostBuilderContext.HostingEnvironment.IsDevelopment())
				siloHost.UseDevelopment(context);
			if (appInfo.IsDockerized)
				siloHost.UseDockerSwarm(context);
			else
				siloHost.UseDevelopmentClustering(context);

			return siloHost;
		}

		private static ISiloBuilder UseDevelopment(this ISiloBuilder siloHost, AppSiloBuilderContext context)
		{
			siloHost
				.AddMemoryGrainStorage(OrleansConstants.PubSubStore)
				.ConfigureServices(services =>
				{
					//services.Configure<GrainCollectionOptions>(options => { options.CollectionAge = TimeSpan.FromMinutes(1.5); });
				})
				.Configure<ClusterMembershipOptions>(options => options.ExpectedClusterSize = 1);

			return siloHost;
		}

		private static ISiloBuilder UseDevelopmentClustering(this ISiloBuilder siloHost, AppSiloBuilderContext context)
		{
			var siloAddress = IPAddress.Loopback;
			var siloPort = context.SiloOptions.SiloPort;
			var gatewayPort = context.SiloOptions.GatewayPort;

			return siloHost
					.UseLocalhostClustering(siloPort: siloPort, gatewayPort: gatewayPort)
				//.AddMemoryGrainStorage(OrleansConstants.GrainPersistenceStorage)
				//.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
				//.ConfigureEndpoints(siloAddress, siloPort, gatewayPort) //, listenOnAnyHostAddress: true)
				;
		}

		private static ISiloBuilder UseDockerSwarm(this ISiloBuilder siloHost, AppSiloBuilderContext context)
		{
			var siloPort = context.SiloOptions.SiloPort;

			var ips = Dns.GetHostAddresses(Dns.GetHostName());
			var defaultIpV4 = ips.First(x => x.AddressFamily == AddressFamily.InterNetwork);

			return siloHost
				.ConfigureEndpoints(
					defaultIpV4,
					siloPort,
					context.SiloOptions.GatewayPort,
					listenOnAnyHostAddress: true
				);
		}
	}
}