namespace Heroes.Server.HeroStat;

public interface IHeroStatsGrain : IGrainWithStringKey, IAppGrainContract
{
	Task<HeroStats> Get();
	Task Set(HeroStats hero);
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeroStats
{
	protected string DebuggerDisplay => $"HeroId: '{HeroId}', WinRate: {WinRate}, BanRate: {BanRate}, TotalGames: {TotalGames}";

	[Id(0)]
	public string HeroId { get; set; }
	[Id(1)]
	public decimal WinRate { get; set; }
	[Id(2)]
	public decimal BanRate { get; set; }
	[Id(3)]
	public int TotalGames { get; set; }

	public override string ToString() => DebuggerDisplay;
}
