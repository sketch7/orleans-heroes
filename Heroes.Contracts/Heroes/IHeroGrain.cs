using Sketch7.Multitenancy.Orleans;
using System.Text.Json.Serialization;

namespace Heroes.Contracts.Heroes;

public interface IHeroGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	Task<Hero> Get();
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Hero
{
	protected string DebuggerDisplay => $"Id: '{Id}', Name: '{Name}', Role: {Role}, Health: {Health}, Popularity: {Popularity}";

	[Id(0)]
	public string Id { get; set; }
	[Id(1)]
	public string Name { get; set; }
	[Id(2)]
	public int Health { get; set; }
	[Id(3)]
	public int Popularity { get; set; }
	[Id(4)]
	public HeroRoleType Role { get; set; }
	[Id(5)]
	public HashSet<string> Abilities { get; set; }

	public override string ToString() => DebuggerDisplay;
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