using Heroes.Contracts;
using Heroes.Contracts.HeroCategories;
using Heroes.Core.Orleans;
using Heroes.Grains.Heroes;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using System.Threading.Tasks;

namespace Heroes.Grains.HeroCategories
{
	public class HeroCategoryState
	{
		public HeroCategory Entity { get; set; }
	}

	public struct HeroCategoryKeyData
	{
		public string Tenant { get; set; }
		public string Key { get; set; }
	}

	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroCategoryGrain : AppGrain<HeroCategoryState>, IHeroCategoryGrain
	{
		private readonly IHeroDataClient _heroDataClient;
		private HeroCategoryKeyData _keyData;

		public HeroCategoryGrain(
			ILogger<HeroGrain> logger,
			IHeroDataClient heroDataClient

		) : base(logger)
		{
			_heroDataClient = heroDataClient;
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();

			// todo: use key data
			var keySplit = PrimaryKey.Split('/');
			_keyData.Tenant = keySplit[1];
			_keyData.Key = keySplit[2];

			if (State.Entity == null)
			{
				var entity = await _heroDataClient.GetHeroCategoryByKey(_keyData.Key);

				if (entity == null)
					return;

				await Set(entity);
			}
		}

		public Task<HeroCategory> Get() => Task.FromResult(State.Entity);

		private Task Set(HeroCategory entity)
		{
			State.Entity = entity;
			return WriteStateAsync();
		}
	}
}