using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Heroes.Api.GraphQLCore.Core;
using Heroes.Api.GraphQLCore.Queries;
using Heroes.Api.GraphQLCore.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Api.GraphQLCore
{
	public static class GraphQLRegistryService
	{
		public static void AddHeroesAppGraphQL(this IServiceCollection services)
		{
			services.AddSingleton<IDocumentExecuter>(new DocumentExecuter());
			services.AddSingleton<IDocumentWriter>(new DocumentWriter(true));

			services.AddScoped<HeroesAppGraphQuery>();
			services.AddScoped<HeroesAppGraphSubscription>();

			services.AddTransient<HeroRoleEnum>();
			services.AddTransient<HeroType>();
			services.AddTransient<HeroStatsType>();
			
			services.AddScoped<ISchema, HeroesAppSchema>();
		}

		public static void SetGraphQLMiddleWare(this IApplicationBuilder app)
		{
			app.UseMiddleware<GraphQLMiddleware>(new GraphQLSettings
			{
				BuildUserContext = ctx => new GraphQLUserContext
				{
					User = ctx.User
				}
			});
		}
	}
}
