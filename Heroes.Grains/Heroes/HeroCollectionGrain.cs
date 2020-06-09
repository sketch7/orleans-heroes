using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Tenancy;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains.Heroes
{
	public class HeroCollectionState
	{
		public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
	}

	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroCollectionGrain : AppGrain<HeroCollectionState>, IHeroCollectionGrain
	{
		private readonly IHeroDataClient _heroDataClient;
		private readonly ITenant _tenant;
		private TenantKeyData _keyData;

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

		public Task<List<string>> GetAll(HeroRoleType? role = null)
		{
			var query = State.HeroKeys.AsQueryable();

			if (role.HasValue)
				query = query.Where(x => x.Value == role);

			var heroIds = query.Select(x => x.Key)
				.ToList();

			return Task.FromResult(heroIds);
		}

		private async Task Set(IEnumerable<Hero> heroes)
		{
			State.HeroKeys = heroes.ToDictionary(x => x.Key, x => x.Role);
			await WriteStateAsync();
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