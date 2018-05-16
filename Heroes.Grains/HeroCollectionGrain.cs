using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Core.Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace Heroes.Grains
{
	public struct HeroCollectionKeyData
	{
		public string Tenant { get; set; }
	}

	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroCollectionGrain : AppGrain<HeroCollectionState>, IHeroCollectionGrain
	{
		private readonly IHeroDataClient _heroDataClient;
		private HeroCollectionKeyData _keyData;

		public HeroCollectionGrain(
			ILogger<HeroCollectionGrain> logger,
			IHeroDataClient heroDataClient

		) : base(logger)
		{
			_heroDataClient = heroDataClient;
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();

			_keyData.Tenant = PrimaryKey.Split('\\')[1];

			if (State.HeroKeys == null)
				await FetchFromRemote();
		}

		public async Task Set(List<Hero> heroes)
		{
			State.HeroKeys = heroes.ToDictionary(x => x.Key, x => x.Role);
			await WriteStateAsync();
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
				var heroGrain = GrainFactory.GetHeroGrain(_keyData.Tenant, heroId);
				promises.Add(heroGrain.Get());
			}

			var result = await Task.WhenAll(promises);
			return result.ToList();
		}

		private async Task FetchFromRemote()
		{
			var heroes = await _heroDataClient.GetAll();
			await Set(heroes);
		}

	}
}