using Orleans;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Contracts.Stats
{
	public interface IHeroStatsGrain : IGrainWithStringKey
	{
		Task<HeroStats> Get();
		Task Set(HeroStats hero);
	}

	[DebuggerDisplay("${DebuggerDisplay, nq}")]
	public class HeroStats
	{
		protected string DebuggerDisplay => $"HeroId: '{HeroId}', WinRate: {WinRate}, BanRate: {BanRate}, TotalGames: {TotalGames}";
		public string HeroId { get; set; }
		public decimal WinRate { get; set; }
		public decimal BanRate { get; set; }
		public int TotalGames { get; set; }

		public override string ToString() => DebuggerDisplay;
	}
}