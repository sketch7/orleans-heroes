using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heroes.Api.Infrastructure
{
	public static class OrleansConfig
	{
		public static IServiceCollection ConfigureClusterClient(this IServiceCollection services, int initializeAttemptsBeforeFailing = 7)
		{
			try
			{
				var clientConfig = ClientConfiguration.LocalhostSilo();
				var clientAsync = InitializeWithRetries(clientConfig, initializeAttemptsBeforeFailing);
				clientAsync.Wait();
				services.AddSingleton(clientAsync.Result);

			}
			catch (Exception ex)
			{
				Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

				Console.ReadLine();
			}
			return services;
		}

		private static async Task<IClusterClient> InitializeWithRetries(ClientConfiguration clientConfig, int initializeAttemptsBeforeFailing)
		{
			int attempt = 0;
			while (true)
			{
				try
				{
					var client = new ClientBuilder()
						.UseConfiguration(clientConfig)
						.Build();

					await client.Connect();
					Console.WriteLine("Client successfully connect to silo host");
					return client;
				}
				catch (SiloUnavailableException ex)
				{
					attempt++;
					if (attempt > initializeAttemptsBeforeFailing)
					{
						Console.WriteLine(ex.Message);
						throw;
					}
					var delay = 2 * attempt;
					Console.WriteLine(
						"Cluster client failed to connect. Attempt {attempt} of {initializeAttemptsBeforeFailing}. Retrying in {delay}s.",
						attempt, initializeAttemptsBeforeFailing, delay);
					Thread.Sleep(TimeSpan.FromSeconds(delay));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					throw;
				}
			}

		}
	}
}