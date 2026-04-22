using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.HeroCategory;

[GenerateSerializer]
public sealed class HeroCategoryCollectionState
{
	[Id(0)]
	public HashSet<string>? HeroCategoryKeys { get; set; }
}

public sealed class HeroCategoryCollectionGrain(
	ILogger<HeroCategoryCollectionGrain> logger,
	IHeroDataClient heroDataClient,
	[PersistentState("heroCategoryCollection", OrleansConstants.GrainMemoryStorage)]
	IPersistentState<HeroCategoryCollectionState> state
) : AppGrain<HeroCategoryCollectionState>(logger, state), IHeroCategoryCollectionGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		if (State.HeroCategoryKeys is null)
			await FetchFromRemote();
	}

	public Task<List<string>> GetAll()
		=> Task.FromResult((State.HeroCategoryKeys ?? []).ToList());

	private async Task Set(List<string> keys)
	{
		State.HeroCategoryKeys = [.. keys];
		await WriteStateAsync();
	}

	private async Task FetchFromRemote()
	{
		var heroCategories = await heroDataClient.GetAllHeroCategory();
		await Set(heroCategories.Select(x => x.Id).ToList());
	}
}
