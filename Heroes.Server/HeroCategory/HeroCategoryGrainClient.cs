namespace Heroes.Server.HeroCategory;

public interface IHeroCategoryGrainClient
{
	Task<HeroCategoryModel> Get(string key);
	Task<List<HeroCategoryModel>> GetAll();
}

public sealed class HeroCategoryGrainClient : IHeroCategoryGrainClient
{
	private readonly IGrainFactory _grainFactory;
	private readonly ITenantAccessor<AppTenant> _tenantAccessor;

	public HeroCategoryGrainClient(
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

	public Task<HeroCategoryModel> Get(string key)
		=> _grainFactory.GetHeroCategoryGrain(TenantKey, key).Get();

	public async Task<List<HeroCategoryModel>> GetAllByRefs(ICollection<string> keys)
	{
		if (keys?.Count == 0)
			return null;

		var categories = await keys.SelectAsync(key => _grainFactory.GetHeroCategoryGrain(TenantKey, key).Get());
		return categories.ToList();
	}

	public async Task<List<HeroCategoryModel>> GetAll()
	{
		var keys = await _grainFactory.GetHeroCategoryCollectionGrain(TenantKey).GetAll();
		return await GetAllByRefs(keys);
	}
}
