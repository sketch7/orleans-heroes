using Heroes.Contracts.Grains;
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
			var config = ClientConfiguration.LocalhostSilo();
			StartClient(config).Wait();

			//var taskHasErrors = UseOldImplementation(config);
			//taskHasErrors.Wait();

			//if (taskHasErrors.Result)
			//{
			//	return 1;
			//}

			Console.WriteLine("Press Enter to terminate...");
			Console.ReadLine();
			return 0;
		}

		private static async Task<bool> UseOldImplementation(ClientConfiguration config)
		{
			try
			{
				InitializeWithRetries(config, 7);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

				Console.ReadLine();

				return true;
			}

			await AddHeroes();
			GetHero();

			return false;
		}

		static async Task StartClient(ClientConfiguration config)
		{
			IClusterClient client = null;
			try
			{
				client = await InitializeWithRetriesAsync(config, 7);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

				Console.ReadLine();
			}

			await AddHeroes(client);
			GetHero(client);
			GetAll(client);
		}

		private static async Task<IClusterClient> InitializeWithRetriesAsync(ClientConfiguration config, int initializeAttemptsBeforeFailing)
		{
			int attempt = 0;
			IClusterClient client;
			while (true)
			{
				try
				{
					//GrainClient.Initialize(config);
					client = new ClientBuilder()
						.UseConfiguration(config)
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
					Thread.Sleep(TimeSpan.FromSeconds(2));
				}
			}

			return client;
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
			await hero.Set(new Hero { Name = "Rengar", Key = "rengar", Role = HeroRoleType.Assassin });
		}

		private static async void GetHero()
		{
			var grain = GrainClient.GrainFactory.GetGrain<IHeroGrain>("rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static Task AddHeroes()
		{
			var list = GrainClient.GrainFactory.GetGrain<IHeroCollectionGrain>(0);
			return list.Set(new Hero { Name = "Rengar", Key = "rengar", Role = HeroRoleType.Assassin },
				new Hero { Name = "Kha 'Zix", Key = "kha-zix", Role = HeroRoleType.Assassin },
				new Hero { Name = "Singed", Key = "singed", Role = HeroRoleType.Tank }
			);
		}


		private static async void GetHero(IClusterClient client)
		{
			var grain = client.GetGrain<IHeroGrain>("rengar");
			var hero = await grain.Get();
			Console.WriteLine($"{hero.Name} is awaken!");
		}

		private static Task AddHeroes(IClusterClient client)
		{
			var list = client.GetGrain<IHeroCollectionGrain>(0);
			return list.Set(new Hero { Name = "Rengar", Key = "rengar", Role = HeroRoleType.Assassin },
				new Hero { Name = "Kha 'Zix", Key = "kha-zix", Role = HeroRoleType.Support },
				new Hero { Name = "Singed", Key = "singed", Role = HeroRoleType.Tank },
				new Hero { Name = "Kha 'Zix", Key = "kha-zix", Role = HeroRoleType.Assassin }
				);
		}

		private static async void GetAll(IClusterClient client)
		{
			var grain = client.GetGrain<IHeroCollectionGrain>(0);
			var heroes = await grain.GetAll();

			foreach (var hero in heroes){
				Console.WriteLine(hero);
			}
		}

	}
}
