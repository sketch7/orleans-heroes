using System.Threading.Tasks;

namespace Heroes.Contracts.Stats
{
	public interface IHeroStatsGrainClient
	{
		Task<HeroStats> Get(string heroId);
		Task Set(HeroStats heroStats);
	}
}