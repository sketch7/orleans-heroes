using Microsoft.AspNetCore.Http;
using System;

namespace Heroes.Server.Gql.Core
{
	public class GqlMiddlewareOptions
	{
		public PathString Path { get; set; } = "/graphql";
		public Func<HttpContext, object> BuildUserContext { get; set; }
	}
}