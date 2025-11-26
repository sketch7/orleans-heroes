namespace Heroes.Contracts.Heroes;

public interface IHeroAbilitiesGrain : IGrainWithStringKey
{
	Task<List<HeroAbility>> Get();
	Task Set(List<HeroAbility> heroAbilities);
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeroAbility
{
	protected string DebuggerDisplay => $"Id: '{Id}', HeroId: '{HeroId}', Name: '{Name}', Damage: {Damage}, DamageType: {DamageType}";

	[Id(0)]
	public string Id { get; set; }
	[Id(1)]
	public string HeroId { get; set; }
	[Id(2)]
	public string Name { get; set; }
	[Id(3)]
	public int Damage { get; set; }
	[Id(4)]
	public DamageType DamageType { get; set; }
	public override string ToString() => DebuggerDisplay;
}

public enum DamageType
{
	None,
	AttackDamage,
	MagicDamage
}