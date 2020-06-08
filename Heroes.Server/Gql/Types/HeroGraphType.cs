using GraphQL.Types;
using Heroes.Contracts.Heroes;

namespace Heroes.Server.Gql.Types
{
	public class HeroGraphType : ObjectGraphType<Hero>
	{
		public HeroGraphType()
		{
			Name = "Hero";
			Description = "A Hero character with unique abilities to play with.";

			Field<StringGraphType>("id", resolve: ctx => ctx.Source.Key);
			Field(x => x.Key).Description("Hero unique key.");
			Field(x => x.Name).Description("Hero name.");
			Field(x => x.Popularity).Description("Hero popularity.");
			Field<HeroRoleGraphType>("role", "Hero role type e.g. assassin");
			Field<ListGraphType<StringGraphType>>("abilities", resolve: ctx => ctx.Source.Abilities, description: "Hero abilities.");
		}
	}
}