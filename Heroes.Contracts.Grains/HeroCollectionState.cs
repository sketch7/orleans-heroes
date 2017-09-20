using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains
{
	public class HeroCollectionState : IGrainState
	{
		public Dictionary<string, HeroRoleType> HeroKeys { get; set; }
		public object State { get; set; }
		public string ETag { get; set; }
	}

	public interface IHeroCollectionGrain : IGrainWithIntegerKey
	{
		Task Set(params Hero[] heroes);
		Task<List<Hero>> GetAll(HeroRoleType? role);
	}
}