using GraphQL.Types;
using Heroes.Api.GraphQLCore.Types;
using Heroes.Contracts.Grains;
using Orleans;

namespace Heroes.Api.GraphQLCore.Queries
{
    public class HeroesAppGraphQuery : ObjectGraphType<object>
    {
        public HeroesAppGraphQuery(IClusterClient clusterClient)
        {
            Name = "Hero Query";
            Field<HeroType>(
                name: "hero",
                description: "hero full object",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key", Description = "Unique key for specific hero"}
                    ),
                resolve: context => clusterClient.GetHeroGrain(context.GetArgument<string>("id"))
                );
        }
    }
}
