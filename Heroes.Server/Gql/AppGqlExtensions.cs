using GraphQL;
using GraphQL.Execution;
using Heroes.Server.Gql.Core;
using Heroes.Server.Gql.Types;

namespace Heroes.Server.Gql;

public static class AppGqlExtensions
{
	public static void AddAppGraphQL(this IServiceCollection services)
	{
		services.AddGraphQL(builder => builder
			.AddNewtonsoftJson()
			.AddDataLoader()
			.AddAutoSchema<AppSchema>()
			.AddExecutionStrategySelector<GraceDefaultExecutionStrategySelector>()
			.AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User })
		);

		services.AddSingleton<AppGraphQuery>();
		services.AddScoped<AppGraphSubscription>();

		services.AddSingleton<HeroRoleGraphType>();
		services.AddSingleton<HeroGraphType>();
		services.AddSingleton<HeroCategoryGraphType>();
		services.AddSingleton<HeroStatsGraphType>();
	}
}

// todo: only needed for hack due Grace IEnumerable doesnt seem to be optional
public class GraceDefaultExecutionStrategySelector : DefaultExecutionStrategySelector
{
	public GraceDefaultExecutionStrategySelector() : base()
	{
	}
}