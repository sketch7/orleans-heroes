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
			var heroGrain = GrainClient.GrainFactory.GetGrain<IHeroGrain>(hero.Key);
			heroGrain.Set(hero);
			State.Heroes.Add(hero);
			return WriteStateAsync();
		}

		public Task SetAll(params Hero[] heroes)
		{
			foreach (var hero in heroes)
			{
				var heroGrain = GrainClient.GrainFactory.GetGrain<IHeroGrain>(hero.Key);
				heroGrain.Set(hero);
				State.Heroes.Add(hero);
			}

			return WriteStateAsync();
		}

		public Task<List<Hero>> GetAll()
		{
			return Task.FromResult(State.Heroes);
		}

		public Task<Hero> GetById(string key)
		{
			var hero = State.Heroes.FirstOrDefault(x => x.Key == key);
			return Task.FromResult(hero);
		}
	}
}