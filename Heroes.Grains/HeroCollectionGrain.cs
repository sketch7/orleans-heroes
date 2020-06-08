using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Tenancy;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	public class HeroCollectionState
	{
		public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
	}

	public struct HeroCollectionKeyData
	{
		public string Tenant { get; set; }
	}

	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroCollectionGrain : AppGrain<HeroCollectionState>, IHeroCollectionGrain
	{
		private readonly IHeroDataClient _heroDataClient;
		private readonly ITenant _tenant;
		private HeroCollectionKeyData _keyData;

		public HeroCollectionGrain(
			ILogger<HeroCollectionGrain> logger,
			IHeroDataClient heroDataClient,
			ITenant tenant

		) : base(logger)
		{
			_heroDataClient = heroDataClient;
			_tenant = tenant;
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();

			_keyData.Tenant = _tenant.Key;
			//_keyData.Tenant = PrimaryKey.Split('/')[1];

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

			var tasks = new List<Task<Hero>>();
			foreach (var heroId in heroIds)
			{
				var heroGrain = GrainFactory.GetHeroGrain(_keyData.Tenant, heroId);
				tasks.Add(heroGrain.Get());
			}

			var result = await Task.WhenAll(tasks);
			return result.ToList();
		}

		private async Task FetchFromRemote()
		{
			var heroes = await _heroDataClient.GetAll();
			await Set(heroes.OrderBy(x => x.Name)
				.ThenBy(x => x.Role)
				.ToList()
			);
		}

	}
}