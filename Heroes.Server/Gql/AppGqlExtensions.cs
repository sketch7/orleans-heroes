using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Heroes.Server.Gql.Core;
using Heroes.Server.Gql.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Server.Gql
{
	public static class AppGqlExtensions
	{
		public static void AddAppGraphQL(this IServiceCollection services)
		{
			services.AddSingleton<IDocumentExecuter>(new DocumentExecuter());
			services.AddSingleton<IDocumentWriter>(new DocumentWriter(true));

			services.AddScoped<AppGraphQuery>();
			services.AddScoped<AppGraphSubscription>();

			services.AddTransient<HeroRoleEnum>();
			services.AddTransient<HeroType>();
			services.AddTransient<HeroStatsType>();

			services.AddScoped<ISchema, AppSchema>();
		}

		public static void UseAppGraphQLMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<GraphQLMiddleware>(new GqlMiddlewareOptions
			{
				BuildUserContext = ctx => new GraphQLUserContext
				{
					User = ctx.User
				}
			});
		}
	}
}
