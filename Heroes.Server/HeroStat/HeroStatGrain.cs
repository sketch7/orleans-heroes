using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace Heroes.Server.HeroStat;

// Stats seed data (LoL heroes only)
file static class StatsData
{
	public static readonly List<HeroStats> All =
	[
		new HeroStats { HeroId = "rengar", BanRate = 20.75M, WinRate = 50.2M, TotalGames = 60 },
		new HeroStats { HeroId = "kha-zix", BanRate = 32M, WinRate = 60.2M, TotalGames = 75 },
		new HeroStats { HeroId = "singed", BanRate = 10M, WinRate = 75.2M, TotalGames = 100 },
	];
}

[GenerateSerializer]
public sealed class HeroStatsState
{
	[Id(0)]
	public HeroStats HeroStats { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public sealed class HeroStatGrain : AppGrain<HeroStatsState>, IHeroStatsGrain
{
	public HeroStatGrain(ILogger<HeroStatGrain> logger) : base(logger)
	{
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		if (State.HeroStats == null)
			State.HeroStats = StatsData.All.SingleOrDefault(x => x.HeroId == PrimaryKey);
	}

	public Task<HeroStats> Get()
		=> Task.FromResult(State.HeroStats);

	public Task Set(HeroStats hero)
	{
		State.HeroStats = hero;
		return WriteStateAsync();
	}
}
