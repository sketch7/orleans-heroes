using Orleans.Providers;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.Hero;

[GenerateSerializer]
public sealed class HeroCollectionState
{
	[Id(0)]
	public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public sealed class HeroCollectionGrain : AppGrain<HeroCollectionState>, IHeroCollectionGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;

	public HeroCollectionGrain(
		ILogger<HeroCollectionGrain> logger,
		IHeroDataClient heroDataClient
	) : base(logger)
	{
		_heroDataClient = heroDataClient;
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		if (State.HeroKeys == null)
			await FetchFromRemote();
	}

	public Task<List<string>> GetAll(HeroRoleType? role = null)
	{
		var query = State.HeroKeys.AsQueryable();

		if (role.HasValue)
			query = query.Where(x => x.Value == role);

		return Task.FromResult(
			query.Select(x => x.Key).ToList()
		);
	}

	private async Task Set(IEnumerable<HeroModel> heroes)
	{
		State.HeroKeys = heroes.ToDictionary(x => x.Id, x => x.Role);
		await WriteStateAsync();
	}

	private async Task FetchFromRemote()
	{
		var heroes = await _heroDataClient.GetAll();
		await Set(heroes.OrderBy(x => x.Name).ThenBy(x => x.Role));
	}
}
