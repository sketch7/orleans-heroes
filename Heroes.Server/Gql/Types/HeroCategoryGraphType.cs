namespace Heroes.Server.Gql.Types;

using Hero = Heroes.Server.Hero.Hero;
using HeroCategory = Heroes.Server.HeroCategory.HeroCategory;

public class HeroCategoryGraphType : ObjectGraphType<HeroCategory>
{
	public HeroCategoryGraphType()
	{
		Name = "HeroCategory";
		Description = "A Hero category grouping.";

		Field(x => x.Id).Description("Hero category unique key.");
		Field(x => x.Title).Description("Hero Category title.");

		Field<ListGraphType<HeroGraphType>, List<Hero>?>("heroes")
			.ResolveAsync(async ctx =>
			{
				var client = ((GraphQLUserContext)ctx.UserContext).HeroGrainClient;
				return await client.GetAllByRefs(ctx.Source.Heroes);
			})
			.Description("Heroes in category")
			;
	}
}