using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;

namespace Heroes.Server.Gql.Types;

public class HeroCategoryGraphType : ObjectGraphType<HeroCategory>
{
	public HeroCategoryGraphType(
		IHeroGrainClient heroGrainClient
	)
	{
		Name = "HeroCategory";
		Description = "A Hero category grouping.";

		Field(x => x.Id).Description("Hero category unique key.");
		Field(x => x.Title).Description("Hero Category title.");

		Field<ListGraphType<HeroGraphType>, List<Hero>>("heroes")
			.ResolveAsync(ctx => heroGrainClient.GetAllByRefs(ctx.Source.Heroes))
			.Description("Heroes in category")
			;
	}
}