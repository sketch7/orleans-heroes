using Heroes.Contracts;
using Heroes.Contracts.HeroCategories;
using Heroes.Core.Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Grains.HeroCategories;

[GenerateSerializer]
public class HeroCategoryCollectionState
{
	[Id(0)]
	public HashSet<string> HeroCategoryKeys { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public class HeroCategoryCollectionGrain : AppGrain<HeroCategoryCollectionState>, IHeroCategoryCollectionGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;

	public HeroCategoryCollectionGrain(
		ILogger<HeroCategoryCollectionGrain> logger,
		IHeroDataClient heroDataClient
	) : base(logger)
	{
		_heroDataClient = heroDataClient;
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

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
		await Set(heroCategories.Select(x => x.Id).ToList());
	}
}