using System;
using System.Linq;
using System.Net;
using Heroes.Contracts.Grains;
using Heroes.Core;
using Heroes.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Heroes.SiloHost.ConsoleApp.Infrastructure
{
	public static class SiloBuilderExtensions
	{
		public static ISiloHostBuilder UseHeroConfiguration(this ISiloHostBuilder siloHost, IAppInfo appInfo, HostingEnvironment hostingEnv)
		{
			siloHost
				.AddMemoryGrainStorage(OrleansConstants.GrainMemoryStorage)
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = appInfo.ClusterId;
					options.ServiceId = appInfo.Name;
				});

			if (hostingEnv.IsDev)
				siloHost.UseDevelopment();
			if (appInfo.IsDockerized)
				siloHost.UseDockerSwarm();
			else
				siloHost.UseDevelopmentClustering();

			return siloHost;
		}

		private static ISiloHostBuilder UseDevelopment(this ISiloHostBuilder siloHost)
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

		private static ISiloHostBuilder UseDevelopmentClustering(this ISiloHostBuilder siloHost)
		{
			var siloAddress = IPAddress.Loopback;
			var siloPort = 11111;
			var gatewayPort = 30000;

			return siloHost
					.AddMemoryGrainStorage(OrleansConstants.GrainPersistenceStorage)
					.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
					.ConfigureEndpoints(siloAddress, siloPort, gatewayPort) //, listenOnAnyHostAddress: true)
				;
		}

		private static ISiloHostBuilder UseDockerSwarm(this ISiloHostBuilder siloHost)
		{
			var ips = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;
			var defaultIp = ips.FirstOrDefault();

			return siloHost
				.ConfigureEndpoints(
					defaultIp,
					RandomUtils.GenerateNumber(30001, 30100), // todo: really needed random?
					RandomUtils.GenerateNumber(20001, 20100), // todo: really needed random?
					listenOnAnyHostAddress: true
				);
		}

	}
}