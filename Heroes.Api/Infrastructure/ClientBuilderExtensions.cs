using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Heroes.Clients.Heroes;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;

namespace Heroes.Api.Infrastructure
{
	public static class ClientBuilderExtensions
	{
		public static IServiceCollection UseOrleansClient(this IServiceCollection services, ClientBuilderContext context)
		{
			if (context == null)
				throw new ArgumentNullException($"{nameof(context)}");
			if (context.AppInfo == null)
				throw new ArgumentNullException($"{nameof(context.AppInfo)}");

			try
			{
				var client = InitializeWithRetries(context).Result;
				services.AddSingleton(client);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

				Console.ReadLine();
			}
			return services;
		}

		private static async Task<IClusterClient> InitializeWithRetries(ClientBuilderContext context)
		{
			var attempt = 0;
			var stopwatch = Stopwatch.StartNew();
			var clientClusterConfig = new ClientConfiguration();

			await Task.Delay(TimeSpan.FromSeconds(clientClusterConfig.DelayInitialConnectSeconds));

			var clientConfig = new ClientBuilder()
				.UseConfiguration(context)
				.ConfigureApplicationParts(x =>
				{
					x.AddApplicationPart(typeof(IHeroClient).Assembly).WithReferences();
					x.AddApplicationPart(typeof(HeroClient).Assembly).WithReferences();
				});

			context.ConfigureClientBuilder?.Invoke(clientConfig);

			var client = clientConfig.Build();
			await client.Connect(async ex =>
			{
				attempt++;
				if (attempt > clientClusterConfig.ConnectionRetry.TotalRetries)
				{
					Console.WriteLine(ex.Message);
					return false;
				}

				var delay = RandomUtils.GenerateNumber(clientClusterConfig.ConnectionRetry.MinDelaySeconds, clientClusterConfig.ConnectionRetry.MaxDelaySeconds);
				Console.WriteLine("Client cluster {0} failed to connect. Attempt {1} of {2}. Retrying in {3}s.",
					context.ClusterId, attempt, clientClusterConfig.ConnectionRetry.TotalRetries, delay);
				await Task.Delay(TimeSpan.FromSeconds(delay));
				return true;
			});

			//context.Logger.LogInformation("Client cluster connected successfully to silo {clusterId} in {timeTaken:#.##}s.",
			Console.WriteLine("Client cluster connected successfully to silo {0} in {1:#.##}s.",
				context.ClusterId, stopwatch.Elapsed.TotalSeconds);
			return client;
		}

		public static IClientBuilder UseConfiguration(
			this IClientBuilder clientBuilder,
			ClientBuilderContext context
		)
		{
			if (!context.AppInfo.IsDockerized)
			{
				var siloAddress = IPAddress.Loopback;
				const int gatewayPort = 30000; // 10400
				clientBuilder.UseStaticClustering(new IPEndPoint(siloAddress, gatewayPort));
			}

			return clientBuilder.Configure<ClusterOptions>(config =>
				{
					config.ClusterId = context.ClusterId;
					config.ServiceId = context.ServiceId;
				});
		}

	}
}