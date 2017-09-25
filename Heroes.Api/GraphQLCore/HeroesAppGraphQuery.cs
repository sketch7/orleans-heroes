using System;
using GraphQL.Types;
using Heroes.Api.GraphQLCore.Types;
using Heroes.Contracts.Grains;
using Orleans;

namespace Heroes.Api.GraphQLCore.Queries
{
	public class HeroesAppGraphQuery : ObjectGraphType
	{
		public HeroesAppGraphQuery(IClusterClient clusterClient)
		{
			Name = "HeroQuery";
			Field<HeroType>(
				name: "hero",
				description: "hero full object",
				arguments: new QueryArguments(
					new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key", Description = "Unique key for specific hero" }
					),
				resolve: context =>
				{
					var grain = clusterClient.GetHeroGrain(context.GetArgument<string>("key"));
					return grain.Get();
				}
				//resolve: context => clusterClient.GetHeroGrain("rengar").Get()
				);
		}
	}
}
