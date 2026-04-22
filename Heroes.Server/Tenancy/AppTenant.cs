using Sketch7.Multitenancy;

namespace Heroes.Server.Tenancy;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record AppTenant : ITenant
{
	private string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}'";

	public required string Key { get; init; }
	public required string Name { get; init; }
}

public interface IAppTenantRegistry : ITenantRegistry<AppTenant> { }

public sealed class AppTenantRegistry : IAppTenantRegistry
{
	public static readonly AppTenant LeagueOfLegends = new() { Key = "lol", Name = "League of Legends" };
	public static readonly AppTenant HeroesOfTheStorm = new() { Key = "hots", Name = "Heroes of the Storm" };

	private readonly Dictionary<string, AppTenant> _tenants;

	public AppTenantRegistry()
	{
		_tenants = new Dictionary<string, AppTenant>
		{
			[LeagueOfLegends.Key] = LeagueOfLegends,
			[HeroesOfTheStorm.Key] = HeroesOfTheStorm,
		};
	}

	/// <summary>Gets a tenant by key, or throws <see cref="KeyNotFoundException"/> if not found.</summary>
	public AppTenant Get(string key)
		=> _tenants.TryGetValue(key, out var tenant)
			? tenant
			: throw new KeyNotFoundException($"Tenant '{key}' not found.");

	/// <summary>Returns all registered tenants.</summary>
	public IEnumerable<AppTenant> GetAll() => _tenants.Values;
}
