using System.Security.Claims;

namespace Heroes.Server.Gql.Core;

public class GraphQLUserContext : Dictionary<string, object?>
{
	public required ClaimsPrincipal User { get; init; }
}