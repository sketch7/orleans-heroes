using Orleans;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Heroes
{
	public class HeroAbilitiesState 
	{
		public List<HeroAbility> HeroAbilities { get; set; }
	}

	public interface IHeroAbilitiesGrain : IGrainWithStringKey
	{
		Task<List<HeroAbility>> Get();
		Task Set(List<HeroAbility> heroAbilities);
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]

	public class HeroAbility
	{
		protected string DebuggerDisplay => $"Id: '{Id}', HeroId: '{HeroId}', Name: '{Name}', Damage: {Damage}, DamageType: {DamageType}";

		public string Id { get; set; }
		public string HeroId { get; set; }
		public string Name { get; set; }
		public int Damage { get; set; }
		public DamageType DamageType { get; set; }
		public override string ToString() => DebuggerDisplay;
	}

	public enum DamageType
	{
		None,
		AttackDamage,
		MagicDamage
	}
}