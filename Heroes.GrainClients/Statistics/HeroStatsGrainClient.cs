using Heroes.Contracts.Stats;

namespace Heroes.GrainClients.Statistics;

public class HeroStatsGrainClient : IHeroStatsGrainClient
{
	private readonly IGrainFactory _grainFactory;

	public HeroStatsGrainClient(
		IGrainFactory grainFactory
	)
	{
		_grainFactory = grainFactory;
	}

	public Task<HeroStats> Get(string heroId)
	{
		var grain = _grainFactory.GetHeroStatsGrain(heroId);
		return grain.Get();
	}

	public Task Set(HeroStats heroStats)
	{
		var grain = _grainFactory.GetHeroStatsGrain(heroStats.HeroId);
		return grain.Set(heroStats);
	}
}