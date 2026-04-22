using GraphQL;
using System.Security.Claims;

namespace Heroes.Server.Gql;

public static class ResolveFieldContextExtensions
{
	extension(IResolveFieldContext ctx)
	{
		public AppGqlUserContext AppUserContext => (AppGqlUserContext)ctx.UserContext;
	}
}

public sealed class AppGqlUserContext : Dictionary<string, object?>
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
