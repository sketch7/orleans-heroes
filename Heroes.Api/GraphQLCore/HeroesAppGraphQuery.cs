using GraphQL.Types;
using Heroes.Api.GraphQLCore.Types;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Stats;

namespace Heroes.Api.GraphQLCore.Queries
{
	public class HeroesAppGraphQuery : ObjectGraphType
	{
		public HeroesAppGraphQuery(
			IHeroClient heroClient,
			IHeroStatsClient heroStatsClient
			)
		{
			Name = "AppQueries";

			Field<HeroType>(
				name: "hero",
				description: "hero full object",
				arguments: new QueryArguments(
					new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key", Description = "Unique key for specific hero" }
					),
				resolve: context =>
				{
					var result = heroClient.Get(context.GetArgument<string>("key"));
					return result;
				}
			);

			Field<ListGraphType<HeroType>>(
				name: "heroes",
				description: "heroes list",
				arguments: new QueryArguments(
					new QueryArgument<HeroRoleEnum> { Name = "role", Description = "filtering heroes by role." }
				),
				resolve: context =>
				{
					var role = context.GetArgument<int?>("role");
					var result = heroClient.GetAll((HeroRoleType?)role);
					return result;
				}
			);

			Field<HeroStatsType>(
				name: "herostats",
				description: "view all hero stats",
				arguments: new QueryArguments(
					new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key", Description = "Unique key for specific hero" }
				),
				resolve: context =>
				{
					var result = heroStatsClient.Get(context.GetArgument<string>("key"));
					return result;
				}

			);
		}
	}
}
