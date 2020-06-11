using Heroes.Contracts;
using Heroes.Contracts.HeroCategories;
using Heroes.Core.Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains.HeroCategories
{
	public class HeroCategoryCollectionState
	{
		public HashSet<string> HeroCategoryKeys { get; set; }
	}

	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroCategoryCollectionGrain : AppGrain<HeroCategoryCollectionState>, IHeroCategoryCollectionGrain
	{
		private readonly IHeroDataClient _heroDataClient;
		private TenantKeyData _keyData;

		public HeroCategoryCollectionGrain(
			ILogger<HeroCategoryCollectionGrain> logger,
			IHeroDataClient heroDataClient
		) : base(logger)
		{
			_heroDataClient = heroDataClient;
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();

			_keyData = this.ParseKey<TenantKeyData>(TenantKeyData.Template);

			if (State.HeroCategoryKeys == null)
				await FetchFromRemote();
		}

		public async Task Set(List<string> keys)
		{
			State.HeroCategoryKeys = new HashSet<string>(keys);
			await WriteStateAsync();
		}

		public Task<List<string>> GetAll()
			=> Task.FromResult(State.HeroCategoryKeys.ToList());

		private async Task FetchFromRemote()
		{
			var heroCategories = await _heroDataClient.GetAllHeroCategory();
			await Set(heroCategories.Select(x => x.Key).ToList());
		}
	}
}