using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Heroes.Client.ConsoleApp
{
	public static class Program
	{
		static async Task<int> Main(string[] args)
		{
			try
			{
				using (var client = await BuildClient())
				{
					await GetHero(client);
					await GetAll(client);
					Console.ReadKey();
				}
				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Console.ReadKey();
				return 1;
			}
		}

		private static async Task<IClusterClient> BuildClient()
		{
			var client = new ClientBuilder()
					.UseLocalhostClustering(30001, clusterId: "dev", serviceId: "heroes")
					.ConfigureApplicationParts(parts => parts
						.AddApplicationPart(typeof(IHeroGrain).Assembly).WithReferences()
					)
					.ConfigureLogging(logging => logging.AddConsole())
					.Build();

			await client.Connect(CreateRetryFilter());

			Console.WriteLine("Client successfully connect to silo host");
			return client;
		}

		private static async Task GetHero(IGrainFactory grainFactory)
		{
			var grain = grainFactory.GetHeroGrain("lol", "rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static async Task GetAll(IGrainFactory grainFactory)
		{
			var grain = grainFactory.GetHeroCollectionGrain("lol");
			var heroes = await grain.GetAll();

			foreach (var hero in heroes)
			{
				Console.WriteLine(hero);
			}
		}

		private static Func<Exception, Task<bool>> CreateRetryFilter(int maxAttempts = 5)
		{
			var attempt = 0;
			return RetryFilter;

			async Task<bool> RetryFilter(Exception exception)
			{
				attempt++;
				Console.WriteLine($"Cluster client attempt {attempt} of {maxAttempts} failed to connect to cluster.  Exception: {exception}");
				if (attempt > maxAttempts)
				{
					return false;
				}

				await Task.Delay(TimeSpan.FromSeconds(3));
				return true;
			}
		}
	}
}