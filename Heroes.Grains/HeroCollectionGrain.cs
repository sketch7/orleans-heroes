using Heroes.Contracts.Grains;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	[StorageProvider(ProviderName = "MemoryStore")]
	public class HeroCollectionGrain : Grain<HeroCollectionState>, IHeroCollectionGrain
	{
		public async Task Set(params Hero[] heroes)
		{
			var tasks = new List<Task>();
			foreach (var hero in heroes)
			{
				var heroGrain = GrainFactory.GetGrain<IHeroGrain>(hero.Key);
				tasks.Add(heroGrain.Set(hero));

				if (State.HeroKeys.ContainsKey(hero.Key))
				{
					State.HeroKeys[hero.Key] = hero.Role;
				}
				else
				{
					State.HeroKeys.Add(hero.Key, hero.Role);
				}

			}

			tasks.Add(WriteStateAsync());
			await Task.WhenAll(tasks);
			//await WriteStateAsync();
		}

		public override Task OnActivateAsync()
		{
			State.HeroKeys = new Dictionary<string, HeroRoleType>();
			// todo: reload state
			Console.WriteLine("HeroCollectionGrain :: OnActivateAsync :: triggered");
			return Task.WhenAll(
				this.ReadStateAsync(),
				base.OnActivateAsync()
			);
		}

		public Task<List<Hero>> GetAll(HeroRoleType? role)
		{
			var heroIds = State.HeroKeys.Where(x => x.Value == role)
				.Select(x => new { key = x.Key, value = x.Value })
				.ToList();

			return Task.FromResult(new List<Hero>());
		}

	}
}