using GraphQL;
using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Server.Gql.Types;

namespace Heroes.Server.Gql;

public class AppGraphQuery : ObjectGraphType
{
	public AppGraphQuery(
		IHeroGrainClient heroGrainClient,
		IHeroCategoryGrainClient heroCategoryGrainClient
	)
	{
		Name = "AppQueries";

		Field<HeroGraphType, Hero?>("hero")
			.Description("Hero entry.")
			.Argument<StringGraphType>("key", "Unique key.")
			.ResolveAsync(async ctx => await heroGrainClient.Get(ctx.GetArgument<string>("key")))
			;

		Field<ListGraphType<HeroGraphType>, List<Hero>?>("heroes")
			.Description("All available Heroes.")
			.Argument<HeroRoleGraphType>("role", "Filter by role.")
			.ResolveAsync(async ctx =>
			{
				var role = ctx.GetArgument<int?>("role");
				return await heroGrainClient.GetAll((HeroRoleType?)role);
			})
			;

		Field<ListGraphType<HeroCategoryGraphType>, List<HeroCategory>?>("heroCategories")
			.Description("All hero categories.")
			.ResolveAsync(async ctx => await heroCategoryGrainClient.GetAll())
			;

	}
}