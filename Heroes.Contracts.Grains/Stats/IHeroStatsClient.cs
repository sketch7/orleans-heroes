using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Stats
{
	public interface IHeroStatsClient
	{
		Task<HeroStats> Get(string heroId);
		Task Set(HeroStats heroStats);
	}
}