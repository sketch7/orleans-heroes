using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Stats;
using Orleans;
using System.Threading.Tasks;

namespace Heroes.Clients.Statistics
{
	public class HeroStatsClient : IHeroStatsClient
	{
		private readonly IClusterClient _clusterClient;

		public HeroStatsClient(IClusterClient clusterClient)
		{
			_clusterClient = clusterClient;
		}

		public Task<HeroStats> Get(string heroId)
		{
			var grain = _clusterClient.GetHeroStatsGrain(heroId);
			return grain.Get();
		}

		public Task Set(HeroStats heroStats)
		{
			var grain = _clusterClient.GetHeroStatsGrain(heroStats.HeroId);
			return grain.Set(heroStats);
		}
	}
}