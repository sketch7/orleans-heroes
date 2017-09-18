using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains
{
	public class HeroCollectionState : IGrainState
	{
		public List<Hero> Heroes { get; set; }
		public object State { get; set; }
		public string ETag { get; set; }
	}

	public interface IHeroCollectionGrain : IGrainWithIntegerKey
	{
		Task Set(Hero hero);
		Task SetAll(params Hero[] heroes);
		Task<List<Hero>> GetAll();
		Task<Hero> GetById(string key);
	}
}