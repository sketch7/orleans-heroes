﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Heroes.Contracts.Heroes;

public interface IHeroGrain : IGrainWithStringKey, IAppGrainContract
{
	Task<Hero> Get();
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Hero
{
	protected string DebuggerDisplay => $"Id: '{Id}', Name: '{Name}', Role: {Role}, Health: {Health}, Popularity: {Popularity}";

	public string Id { get; set; }
	public string Name { get; set; }
	public int Health { get; set; }
	public int Popularity { get; set; }
	public HeroRoleType Role { get; set; }
	public HashSet<string> Abilities { get; set; }

	public override string ToString() => DebuggerDisplay;
}

[JsonConverter(typeof(StringEnumConverter), true)]
public enum HeroRoleType
{
	Assassin = 1,
	Fighter = 2,
	Mage = 3,
	Support = 4,
	Tank = 5,
	Marksman = 6
}