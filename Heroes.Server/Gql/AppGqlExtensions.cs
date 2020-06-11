using GraphQL.Server;
using GraphQL.Types;
using Heroes.Server.Gql.Core;
using Heroes.Server.Gql.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Server.Gql
{
	public static class AppGqlExtensions
	{
		public static void AddAppGraphQL(this IServiceCollection services)
		{
			services.AddGraphQL(options =>
				{
					options.EnableMetrics = true;
					options.ExposeExceptions = true;
				})
				.AddNewtonsoftJson() // or use AddSystemTextJson for .NET Core 3+
				.AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });

			services.AddSingleton<ISchema, AppSchema>();
			services.AddSingleton<AppGraphQuery>();
			services.AddScoped<AppGraphSubscription>();

			services.AddSingleton<HeroRoleGraphType>();
			services.AddSingleton<HeroGraphType>();
			services.AddSingleton<HeroCategoryGraphType>();
			services.AddSingleton<HeroStatsGraphType>();
		}
	}
}
