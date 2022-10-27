using Heroes.Contracts.HeroCategories;

namespace Heroes.GrainClients.HeroCategories;

public class HeroCategoryGrainClient : IHeroCategoryGrainClient
{
	// todo: get tenant from context
	private const string Tenant = "lol";
	private readonly IGrainFactory _grainFactory;

	public HeroCategoryGrainClient(
		IGrainFactory grainFactory
	)
	{
		_grainFactory = grainFactory;
	}

	public Task<HeroCategory> Get(string key)
	{
		var grain = _grainFactory.GetHeroCategoryGrain(Tenant, key);
		return grain.Get();
	}

	public async Task<List<HeroCategory>> GetAllByRefs(ICollection<string> keys)
	{
		if (keys?.Count == 0)
			return null;

		var heroes = await keys.SelectAsync(key => _grainFactory.GetHeroCategoryGrain(Tenant, key).Get());
		return heroes.ToList();
	}

	public async Task<List<HeroCategory>> GetAll()
	{
		var grain = _grainFactory.GetHeroCategoryCollectionGrain(Tenant);

		var keys = await grain.GetAll();

		return await GetAllByRefs(keys);
	}
}