using GraphQL.Types;
using Heroes.Contracts.Grains.Heroes;

namespace Heroes.Server.Gql.Types
{
	public class HeroGraphType : ObjectGraphType<Hero>
	{
		public HeroGraphType()
		{
			Name = "Hero";
			Description = "A hero object";

			Field<StringGraphType>("id", resolve: ctx => ctx.Source.Key);
			Field(x => x.Key).Description("unique key of a hero.");
			Field(x => x.Name).Description("Hero name.");
			Field(x => x.Popularity).Description("Hero popularity.");
			Field<HeroRoleGraphType>("role", "hero role type.");
			Field<ListGraphType<StringGraphType>>("abilities", resolve: ctx => ctx.Source.Abilities, description: "Hero abilities.");
		}
	}
}