using GraphQL.Types;
using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;

namespace Heroes.Server.Gql.Types
{
	public class HeroCategoryGraphType : ObjectGraphType<HeroCategory>
	{
		public HeroCategoryGraphType(
			IHeroGrainClient heroGrainClient
		)
		{
			Name = "HeroCategory";
			Description = "A Hero category grouping.";

			Field(x => x.Key).Name("id").Description("Hero category unique key.");
			Field(x => x.Title).Description("Hero Category title.");
			Field<ListGraphType<HeroGraphType>>("heroes", resolve: ctx => heroGrainClient.GetAllByRefs(ctx.Source.Heroes), description: "Heroes in category.");
		}
	}
}