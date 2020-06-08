using Heroes.Core.Orleans;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Heroes
{
	public interface IHeroCollectionGrain : IGrainWithStringKey, IAppGrainContract
	{
		Task Set(List<Hero> heroes);
		Task<List<Hero>> GetAll(HeroRoleType? role = null);
	}
}