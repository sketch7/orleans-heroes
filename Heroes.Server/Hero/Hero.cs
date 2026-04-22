using Orleans.Concurrency;
using Sketch7.Multitenancy.Orleans;
using System.Text.Json.Serialization;

namespace Heroes.Server.Hero;

[Alias("IHeroHub")]
public interface IHeroHub
{
	Task Send(string message);
}

[Alias("IHeroGrain")]
public interface IHeroGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<HeroModel?> Get();
}

[Alias("IHeroCollectionGrain")]
public interface IHeroCollectionGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<List<string>> GetAll(HeroRoleType? role = null);
}

[Alias("IHeroAbilitiesGrain")]
public interface IHeroAbilitiesGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<List<HeroAbility>> Get();
	Task Set([Immutable] List<HeroAbility> heroAbilities);
}

[GenerateSerializer]
public sealed record HeroModel
{
	[Id(0)]
	public required string Id { get; init; }
	[Id(1)]
	public required string Name { get; init; }
	[Id(2)]
	public int Health { get; init; }
	[Id(3)]
	public int Popularity { get; init; }
	[Id(4)]
	public HeroRoleType Role { get; init; }
	[Id(5)]
	public HashSet<string> Abilities { get; init; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter<HeroRoleType>))]
public enum HeroRoleType
{
	Assassin = 1,
	Fighter = 2,
	Mage = 3,
	Support = 4,
	Tank = 5,
	Marksman = 6
}

[GenerateSerializer]
public sealed record HeroAbility
{
	[Id(0)]
	public required string Id { get; init; }
	[Id(1)]
	public required string HeroId { get; init; }
	[Id(2)]
	public required string Name { get; init; }
	[Id(3)]
	public int Damage { get; init; }
	[Id(4)]
	public DamageType DamageType { get; init; }
}

public enum DamageType
{
	None,
	AttackDamage,
	MagicDamage
}
