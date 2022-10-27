using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace Heroes.Grains.Heroes;

public class HeroCollectionState
{
	public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public class HeroCollectionGrain : AppGrain<HeroCollectionState>, IHeroCollectionGrain
{
	private readonly IHeroDataClient _heroDataClient;
	private TenantKeyData _keyData;

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

		_keyData = this.ParseKey<TenantKeyData>(TenantKeyData.Template);

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
		State.HeroKeys = heroes.ToDictionary(x => x.Id, x => x.Role);
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