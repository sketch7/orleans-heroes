using Orleans.Concurrency;

namespace Heroes.Server.HeroStat;

[Alias("IHeroStatsGrain")]
public interface IHeroStatsGrain : IGrainWithStringKey, IAppGrainContract
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<HeroStats?> Get();
	Task Set([Immutable] HeroStats hero);
}

[GenerateSerializer]
public sealed record HeroStats
{
	[Id(0)]
	public required string HeroId { get; init; }
	[Id(1)]
	public decimal WinRate { get; init; }
	[Id(2)]
	public decimal BanRate { get; init; }
	[Id(3)]
	public int TotalGames { get; init; }
}
