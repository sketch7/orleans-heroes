using System;
using Microsoft.AspNetCore.Http;

namespace Heroes.Api.GraphQLCore.Core
{
	public class GraphQLSettings
	{
		public PathString Path { get; set; } = "/graphql";
		public Func<HttpContext, object> BuildUserContext { get; set; }
	}
}