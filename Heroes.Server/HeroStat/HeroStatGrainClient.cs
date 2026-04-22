namespace Heroes.Server.HeroStat;

public interface IHeroStatsGrainClient
{
	Task<HeroStats> Get(string heroId);
	Task Set(HeroStats heroStats);
}

public sealed class HeroStatsGrainClient : IHeroStatsGrainClient
{
	private readonly IGrainFactory _grainFactory;

	public HeroStatsGrainClient(IGrainFactory grainFactory)
	{
		_grainFactory = grainFactory;
	}

	public Task<HeroStats> Get(string heroId)
		=> _grainFactory.GetHeroStatsGrain(heroId).Get();

	public Task Set(HeroStats heroStats)
		=> _grainFactory.GetHeroStatsGrain(heroStats.HeroId).Set(heroStats);
}
