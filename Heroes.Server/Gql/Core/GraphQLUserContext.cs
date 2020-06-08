using System.Collections.Generic;
using System.Security.Claims;

namespace Heroes.Server.Gql.Core
{
	public class GraphQLUserContext : Dictionary<string, object>
	{
		public ClaimsPrincipal User { get; set; }
	}
}