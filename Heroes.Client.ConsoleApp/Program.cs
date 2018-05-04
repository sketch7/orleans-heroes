using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Mocks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heroes.Client.ConsoleApp
{
	public class Program
	{
		static int Main(string[] args)
		{
			return RunMainAsync().Result;
		}

		private static async Task<int> RunMainAsync()
		{
			try
			{
				using (var client = await StartClientWithRetries())
				{
					await AddHeroes(client);
					await GetHero(client);
					await GetAll(client);
					Console.ReadKey();
				}

				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return 1;
			}
		}

		private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 7)
		{
			int attempt = 0;
			IClusterClient client;
			while (true)
			{
				try
				{
					client = new ClientBuilder()
						.UseLocalhostClustering()
						.ConfigureApplicationParts(parts => parts
							.AddApplicationPart(typeof(IHeroGrain).Assembly).WithReferences()
						)
						.ConfigureLogging(logging => logging.AddConsole())
						.Build();

					await client.Connect();
					Console.WriteLine("Client successfully connect to silo host");
					break;
				}
				catch (SiloUnavailableException)
				{
					attempt++;
					Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
					if (attempt > initializeAttemptsBeforeFailing)
					{
						throw;
					}
					Thread.Sleep(TimeSpan.FromSeconds(3));
				}
			}

			return client;
		}

		private static async Task GetHero(IClusterClient client)
		{
			var grain = client.GetGrain<IHeroGrain>("rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static Task AddHeroes(IClusterClient client)
		{
			var list = client.GetGrain<IHeroCollectionGrain>(0);
			return list.Set(MockDataService.GetHeroes());
		}

		private static async Task GetAll(IClusterClient client)
		{
			var grain = client.GetGrain<IHeroCollectionGrain>(0);
			var heroes = await grain.GetAll();

			foreach (var hero in heroes)
			{
				Console.WriteLine(hero);
			}
		}

	}
}