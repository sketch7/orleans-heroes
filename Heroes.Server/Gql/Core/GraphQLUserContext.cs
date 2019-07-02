using System.Security.Claims;

namespace Heroes.Server.Gql.Core
{
	public class GraphQLUserContext
	{
		public ClaimsPrincipal User { get; set; }
	}
}