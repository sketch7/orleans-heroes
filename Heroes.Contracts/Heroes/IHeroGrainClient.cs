using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Heroes
{
	public interface IHeroGrainClient
	{
		Task<Hero> Get(string key);
		Task<List<Hero>> GetAll(HeroRoleType? role = null);
	}
}