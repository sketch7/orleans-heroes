using System.Collections.Generic;
using System.Threading.Tasks;
using Heroes.Core.Orleans;
using Orleans;

namespace Heroes.Contracts.Grains.Heroes
{
	public class HeroCollectionState
	{
		public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
	}

	public interface IHeroCollectionGrain : IGrainWithIntegerKey, IAppGrainContract
	{
		Task Set(List<Hero> heroes);
		Task<List<Hero>> GetAll(HeroRoleType? role = null);
	}
}