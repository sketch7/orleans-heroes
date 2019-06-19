using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
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
			var grain = client.GetHeroGrain("lol", "rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static async Task GetAll(IClusterClient client)
		{
			var grain = client.GetHeroCollectionGrain("lol");
			var heroes = await grain.GetAll();

			foreach (var hero in heroes)
			{
				Console.WriteLine(hero);
			}
		}

	}
}