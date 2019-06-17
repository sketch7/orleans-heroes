using Heroes.Contracts.Grains;
using Heroes.Core;
using Heroes.Core.Utils;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace Heroes.Server.Infrastructure
{
	public static class SiloBuilderExtensions
	{
		public static ISiloBuilder UseHeroConfiguration(this ISiloBuilder siloHost, HostBuilderContext hostBuilderContext, IAppInfo appInfo)
		{
			siloHost
				.AddMemoryGrainStorage(OrleansConstants.GrainMemoryStorage)
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = appInfo.ClusterId;
					options.ServiceId = appInfo.Name;
				});

			if (hostBuilderContext.HostingEnvironment.IsDevelopment())
				siloHost.UseDevelopment();
			if (appInfo.IsDockerized)
				siloHost.UseDockerSwarm();
			else
				siloHost.UseDevelopmentClustering();

			return siloHost;
		}

		private static ISiloBuilder UseDevelopment(this ISiloBuilder siloHost)
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

		private static ISiloBuilder UseDevelopmentClustering(this ISiloBuilder siloHost)
		{
			var siloAddress = IPAddress.Loopback;
			var siloPort = GetAvailablePort(11111, 12000);

			var gatewayPort = 30000; // todo: make it configurable

			return siloHost
					.AddMemoryGrainStorage(OrleansConstants.GrainPersistenceStorage)
					.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
					.ConfigureEndpoints(siloAddress, siloPort, gatewayPort) //, listenOnAnyHostAddress: true)
				;
		}

		private static ISiloBuilder UseDockerSwarm(this ISiloBuilder siloHost)
		{
			var ips = Dns.GetHostAddresses(Dns.GetHostName());
			var defaultIp = ips.FirstOrDefault();

			return siloHost
				.ConfigureEndpoints(
					defaultIp,
					RandomUtils.GenerateNumber(30001, 30100), // todo: really needed random?
					RandomUtils.GenerateNumber(20001, 20100), // todo: really needed random?
					listenOnAnyHostAddress: true
				);
		}

		private static int GetAvailablePort(int start, int end)
		{
			for (var port = start; port < end; ++port)
			{
				var listener = TcpListener.Create(port);
				listener.ExclusiveAddressUse = true;
				try
				{
					listener.Start();
					return port;
				}
				catch (SocketException)
				{
				}
				finally
				{
					listener.Stop();
				}
			}

			throw new InvalidOperationException();
		}
	}
}