using Orleans;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Heroes
{
	public class HeroState : IGrainState
	{
		public Hero Hero { get; set; }
		public object State { get; set; }
		public string ETag { get; set; }
	}

	public interface IHeroGrain : IGrainWithStringKey
	{
		Task Set(Hero hero);
		Task<Hero> Get();
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Hero
	{
		protected string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}', Role: {Role}";
		public string Key { get; set; }
		public string Name { get; set; }
		public HeroRoleType Role { get; set; }
		public HashSet<string> Abilities { get; set; }

		public override string ToString() => DebuggerDisplay;
	}

	public enum HeroRoleType
	{
		Assassin,
		Fighter,
		Mage,
		Support,
		Tank,
		Marksman
	}
}