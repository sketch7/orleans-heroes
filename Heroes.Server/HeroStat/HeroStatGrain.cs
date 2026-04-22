namespace Heroes.Server.HeroStat;

// Stats seed data (LoL heroes only)
static file class StatsData
{
	public static readonly List<HeroStats> All =
	[
		new() { HeroId = "rengar", BanRate = 20.75M, WinRate = 50.2M, TotalGames = 60 },
		new() { HeroId = "kha-zix", BanRate = 32M, WinRate = 60.2M, TotalGames = 75 },
		new() { HeroId = "singed", BanRate = 10M, WinRate = 75.2M, TotalGames = 100 },
	];
}

[GenerateSerializer]
public sealed class HeroStatsState
{
	[Id(0)]
	public HeroStats? HeroStats { get; set; }
}

public sealed class HeroStatGrain : AppGrain<HeroStatsState>, IHeroStatsGrain
{
	public HeroStatGrain(
		ILogger<HeroStatGrain> logger,
		[PersistentState("heroStats", OrleansConstants.GrainMemoryStorage)]
		IPersistentState<HeroStatsState> state
	) : base(logger, state)
	{
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		if (State.HeroStats is null)
			State.HeroStats = StatsData.All.SingleOrDefault(x => x.HeroId == PrimaryKey);
	}

	public Task<HeroStats?> Get()
		=> Task.FromResult(State.HeroStats);

	public Task Set(HeroStats hero)
	{
		State.HeroStats = hero;
		return WriteStateAsync();
	}
}
