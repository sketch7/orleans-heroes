using System;
using Heroes.Contracts.Grains;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	[StorageProvider(ProviderName = "MemoryStore")]
	public class HeroCollectionGrain : Grain<HeroCollectionState>, IHeroCollectionGrain
	{
		public Task Set(Hero hero)
		{
			var heroGrain = GrainFactory.GetGrain<IHeroGrain>(hero.Key);
			heroGrain.Set(hero);
			State.HeroKeys.Add(hero.Key);
			return WriteStateAsync();
		}

		public async Task SetAll(params Hero[] heroes)
		{
			var tasks = new List<Task>();
			foreach (var hero in heroes)
			{
				var heroGrain = GrainFactory.GetGrain<IHeroGrain>(hero.Key);
				tasks.Add(heroGrain.Set(hero));
				State.HeroKeys.Add(hero.Key);
			}
			await Task.WhenAll(tasks);
			await WriteStateAsync();
		}

		public override Task OnActivateAsync()
		{
			// todo: reload state
			Console.WriteLine("HeroCollectionGrain :: OnActivateAsync :: triggered");
			return base.OnActivateAsync();
		}

		public Task<List<Hero>> GetAll()
		{
			return Task.FromResult(new List<Hero>());
		}

	}
}