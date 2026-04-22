using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using System.Security.Claims;

namespace Heroes.Server.Gql.Core;

public class GraphQLUserContext : Dictionary<string, object?>
{
	public required ClaimsPrincipal User { get; init; }

	/// <summary>
	/// Captured from the HTTP request scope (where the tenant accessor is set)
	/// so GraphQL resolvers use the correct tenant-aware instances.
	/// </summary>
	public required IHeroGrainClient HeroGrainClient { get; init; }

	/// <inheritdoc cref="HeroGrainClient"/>
	public required IHeroCategoryGrainClient HeroCategoryGrainClient { get; init; }
}