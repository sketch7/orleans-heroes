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
		private const string Source = nameof(HeroCollectionGrain);

		public async Task Set(params Hero[] heroes)
		{
			var promises = new List<Task>();
			foreach (var hero in heroes)
			{
				var heroGrain = GrainFactory.GetHeroGrain(hero.Key);
				promises.Add(heroGrain.Set(hero));

				State.HeroKeys[hero.Key] = hero.Role;
			}
			promises.Add(WriteStateAsync());
			await Task.WhenAll(promises);
		}

		public override Task OnActivateAsync()
		{
			State.HeroKeys = new Dictionary<string, HeroRoleType>();
			Console.WriteLine($"{Source} :: OnActivateAsync");
			return Task.CompletedTask;
		}

		public async Task<List<Hero>> GetAll(HeroRoleType? role = null)
		{
			var query = State.HeroKeys.AsQueryable();

			if (role.HasValue)
				query = query.Where(x => x.Value == role);

			var heroIds = query
				.Select(x => x.Key)
				.ToList();

			var promises = new List<Task<Hero>>();
			foreach (var heroId in heroIds)
			{
				var heroGrain = GrainFactory.GetHeroGrain(heroId);
				promises.Add(heroGrain.Get());
			}

			var result = await Task.WhenAll(promises);
			return result.ToList();
		}

	}
}