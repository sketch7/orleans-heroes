using System.Security.Claims;

namespace Heroes.Api.GraphQLCore.Core
{
    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
    }
}