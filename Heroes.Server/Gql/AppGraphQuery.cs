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

		Field<HeroGraphType, Hero>("hero")
			.Description("Hero entry.")
			.Argument<StringGraphType>("key", "Unique key.")
			.ResolveAsync(ctx =>
			{
				var result = heroGrainClient.Get(ctx.GetArgument<string>("key"));
				return result;
			})
			;

		Field<ListGraphType<HeroGraphType>, List<Hero>>("heroes")
			.Description("All available Heroes.")
			.Argument<HeroRoleGraphType>("role", "Filter by role.")
			.ResolveAsync(ctx =>
			{
				var role = ctx.GetArgument<int?>("role");
				var result = heroGrainClient.GetAll((HeroRoleType?)role);
				return result;
			})
			;

		Field<ListGraphType<HeroCategoryGraphType>, List<HeroCategory>>("heroCategories")
			.Description("All hero categories.")
			.ResolveAsync(ctx =>
			{
				var result = heroCategoryGrainClient.GetAll();
				return result;
			})
			;

	}
}