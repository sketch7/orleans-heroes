using GraphQL;

namespace Heroes.Server.Gql;

public sealed class AppGraphQuery : ObjectGraphType
{
	public AppGraphQuery()
	{
		Name = "AppQueries";

		Field<HeroGraphType, HeroModel?>("hero")
			.Description("Hero entry.")
			.Argument<StringGraphType>("key", "Unique key.")
			.ResolveAsync(async ctx =>
			{
				var client = ctx.AppUserContext.HeroGrainClient;
				return await client.Get(ctx.GetArgument<string>("key"));
			})
			;

		Field<ListGraphType<HeroGraphType>, List<HeroModel>?>("heroes")
			.Description("All available Heroes.")
			.Argument<HeroRoleGraphType>("role", "Filter by role.")
			.ResolveAsync(async ctx =>
			{
				var client = ctx.AppUserContext.HeroGrainClient;
				var role = ctx.GetArgument<int?>("role");
				return await client.GetAll((HeroRoleType?)role);
			})
			;

		Field<ListGraphType<HeroCategoryGraphType>, List<HeroCategoryModel>?>("heroCategories")
			.Description("All hero categories.")
			.ResolveAsync(async ctx =>
			{
				var client = ctx.AppUserContext.HeroCategoryGrainClient;
				return await client.GetAll();
			})
			;
	}
}