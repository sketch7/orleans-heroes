using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Heroes
{
	public interface IHeroClient
	{
		Task<Hero> Get(string key);
		Task<List<Hero>> GetAll(HeroRoleType? role = null);

		Task Set(Hero hero);
		Task Set(List<Hero> heroes);
	}
}