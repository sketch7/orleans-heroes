using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains
{
	public class HeroCollectionState
	{
		public List<string> HeroKeys { get; set; }
	}

	public interface IHeroCollectionGrain : IGrainWithIntegerKey
	{
		Task Set(Hero hero);
		Task SetAll(params Hero[] heroes);
		Task<List<Hero>> GetAll();
	}
}