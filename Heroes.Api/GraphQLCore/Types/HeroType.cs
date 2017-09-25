using GraphQL.Types;
using Heroes.Contracts.Grains.Heroes;
using Orleans;

namespace Heroes.Api.GraphQLCore.Types
{
	public class HeroType : ObjectGraphType<Hero>
    {
        public HeroType(IClusterClient clusterClient)
        {
            Name = "Hero";
            Description = "A hero object";
            Field(x => x.Key).Description("unique key of a hero.");
            Field(x => x.Name).Description("self descriptive.");
            Field<HeroRoleType>(x => x.Role).Description("hero role type.");
        }
    }
}