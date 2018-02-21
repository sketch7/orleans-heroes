using System;
using System.Linq;
using System.Net;
using Heroes.Core;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;

namespace Heroes.SiloHost.ConsoleApp.Infrastructure
{
	public static class ClusterConfig
	{
		public static ClusterConfiguration Configure(IConfiguration config, IAppInfo appInfo, HostingEnvironment hostingEnv)
		{
			var clusterConfig = appInfo.IsDockerized
				? new ClusterConfiguration()
				: ClusterConfiguration.LocalhostPrimarySilo();

			clusterConfig.Globals.ClusterId = $"{appInfo.Name}-{appInfo.Version}";
			clusterConfig.AddMemoryStorageProvider();

			if (hostingEnv.IsDev)
				clusterConfig.UseDevelopment();
			if (appInfo.IsDockerized)
				clusterConfig.UseDockerSwarm();

			return clusterConfig;
		}

		public static ClusterConfiguration UseDevelopment(this ClusterConfiguration clusterConfig)
		{
			//clusterConfig.AddMemoryStorageProvider(Consts.GrainPersistenceStorage);
			clusterConfig.Globals.Application.SetDefaultCollectionAgeLimit(TimeSpan.FromMinutes(10));
			return clusterConfig;
		}

		public static ClusterConfiguration UseDockerSwarm(this ClusterConfiguration clusterConfig)
		{
			clusterConfig.Defaults.PropagateActivityId = true;
			clusterConfig.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, 10400);
			clusterConfig.Defaults.Port = 10300;

			var ips = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;
			var defaultIp = ips.FirstOrDefault();

			clusterConfig.Defaults.HostNameOrIPAddress = defaultIp?.ToString();
			return clusterConfig;
		}

	}
}