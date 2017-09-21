using Orleans;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains
{
	public class HeroStatsState
	{
		public HeroStats HeroStats { get; set; }
	}

	public interface IHeroStatsGrain : IGrainWithStringKey
	{
		Task Set(HeroStats hero);
		Task<HeroStats> Get();
	}

	[DebuggerDisplay("${DebuggerDisplay, nq}")]
	public class HeroStats
	{
		protected string DebuggerDisplay => $"HeroId: '{HeroId}', WinRate: '{WinRate}',  BanRate: '{BanRate}',  TotalGames: '{TotalGames}'";
		public string HeroId { get; set; }
		public decimal WinRate { get; set; }
		public decimal BanRate { get; set; }
		public int TotalGames { get; set; }

		public override string ToString() => DebuggerDisplay;
	}
}