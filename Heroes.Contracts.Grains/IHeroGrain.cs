using Orleans;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains
{
	public interface IHeroGrain : IGrainWithStringKey
	{
		Task Set(Hero hero);
		Task<Hero> Get();
	}

	public class Hero
	{
		public string Key { get; set; }
		public string Name { get; set; }
	}

	public class HeroState : IGrainState
	{
		public Hero Hero { get; set; }

		public object State { get; set; }
		public string ETag { get; set; }
	}
}