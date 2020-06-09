using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Heroes
{
	public interface IHeroGrainClient
	{
		Task<Hero> Get(string key);
		Task<List<Hero>> GetAll(HeroRoleType? role = null);
		Task<List<Hero>> GetAllByRefs(ICollection<string> keys);
	}
}