using Heroes.Contracts.Grains;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Threading;

namespace Heroes.Client.ConsoleApp
{
	public class Program
	{
		static int Main(string[] args)
		{
			var config = ClientConfiguration.LocalhostSilo();
			try
			{
				InitializeWithRetries(config, 5);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

				Console.ReadLine();
				return 1;
			}

			AddHeroes();
			GetHero();

			Console.WriteLine("Press Enter to terminate...");
			Console.ReadLine();
			return 0;
		}

		private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
		{
			int attempt = 0;
			while (true)
			{
				try
				{
					GrainClient.Initialize(config);
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
					Thread.Sleep(TimeSpan.FromSeconds(2));
				}
			}
		}

		private static async void AddHero()
		{
			var hero = GrainClient.GrainFactory.GetGrain<IHeroGrain>("rengar");
			await hero.Set(new Hero { Name = "Rengar", Key = "rengar" });
		}

		private static async void GetHero()
		{
			var grain = GrainClient.GrainFactory.GetGrain<IHeroGrain>("rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static async void AddHeroes()
		{
			var list = GrainClient.GrainFactory.GetGrain<IHeroCollectionGrain>(0);
			await list.SetAll(new Hero { Name = "Rengar", Key = "rengar" },
				new Hero { Name = "Kha 'Zix", Key = "kha-zix" },
				new Hero { Name = "Singed", Key = "singed" }
				);
		}

	}
}
