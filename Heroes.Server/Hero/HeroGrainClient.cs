namespace Heroes.Server.Hero;

public interface IHeroGrainClient
{
	Task<Hero> Get(string key);
	Task<List<Hero>> GetAll(HeroRoleType? role = null);
	Task<List<Hero>> GetAllByRefs(ICollection<string> keys);
}

public sealed class HeroGrainClient : IHeroGrainClient
{
	private readonly IGrainFactory _grainFactory;
	private readonly ITenantAccessor<AppTenant> _tenantAccessor;

	public HeroGrainClient(
		IGrainFactory grainFactory,
		ITenantAccessor<AppTenant> tenantAccessor
	)
	{
		_grainFactory = grainFactory;
		_tenantAccessor = tenantAccessor;
	}

	private string TenantKey
		=> _tenantAccessor.Tenant?.Key
			?? throw new InvalidOperationException("No tenant is set for the current request.");

	public Task<Hero> Get(string key)
		=> _grainFactory.GetHeroGrain(TenantKey, key).Get();

	public async Task<List<Hero>> GetAllByRefs(ICollection<string> keys)
	{
		if (keys?.Count == 0)
			return null;

		var heroes = await keys.SelectAsync(key => _grainFactory.GetHeroGrain(TenantKey, key).Get());
		return heroes.ToList();
	}

	public async Task<List<Hero>> GetAll(HeroRoleType? role = null)
	{
		var keys = await _grainFactory.GetHeroCollectionGrain(TenantKey).GetAll(role);
		return await GetAllByRefs(keys);
	}
}
