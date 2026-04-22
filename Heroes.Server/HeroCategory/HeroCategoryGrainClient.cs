using Heroes.Server.Hero;
using Sketch7.Multitenancy;

namespace Heroes.Server.HeroCategory;

public interface IHeroCategoryGrainClient
{
	Task<HeroCategory> Get(string key);
	Task<List<HeroCategory>> GetAll();
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

	public Task<HeroCategory> Get(string key)
		=> _grainFactory.GetHeroCategoryGrain(TenantKey, key).Get();

	public async Task<List<HeroCategory>> GetAllByRefs(ICollection<string> keys)
	{
		if (keys?.Count == 0)
			return null;

		var categories = await keys.SelectAsync(key => _grainFactory.GetHeroCategoryGrain(TenantKey, key).Get());
		return categories.ToList();
	}

	public async Task<List<HeroCategory>> GetAll()
	{
		var keys = await _grainFactory.GetHeroCategoryCollectionGrain(TenantKey).GetAll();
		return await GetAllByRefs(keys);
	}
}
