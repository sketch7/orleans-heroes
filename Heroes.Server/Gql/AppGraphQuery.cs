using GraphQL;
using Heroes.Server.Gql.Types;

namespace Heroes.Server.Gql
{
	using Hero = Heroes.Server.Hero.Hero;
	using HeroCategory = Heroes.Server.HeroCategory.HeroCategory;

	public class AppGraphQuery : ObjectGraphType
	{
		public AppGraphQuery()
		{
			Name = "AppQueries";

			Field<HeroGraphType, Hero?>("hero")
				.Description("Hero entry.")
				.Argument<StringGraphType>("key", "Unique key.")
				.ResolveAsync(async ctx =>
				{
					var client = ((GraphQLUserContext)ctx.UserContext).HeroGrainClient;
					return await client.Get(ctx.GetArgument<string>("key"));
				})
				;

			Field<ListGraphType<HeroGraphType>, List<Hero>?>("heroes")
				.Description("All available Heroes.")
				.Argument<HeroRoleGraphType>("role", "Filter by role.")
				.ResolveAsync(async ctx =>
				{
					var client = ((GraphQLUserContext)ctx.UserContext).HeroGrainClient;
					var role = ctx.GetArgument<int?>("role");
					return await client.GetAll((HeroRoleType?)role);
				})
				;

			Field<ListGraphType<HeroCategoryGraphType>, List<HeroCategory>?>("heroCategories")
				.Description("All hero categories.")
				.ResolveAsync(async ctx =>
				{
					var client = ((GraphQLUserContext)ctx.UserContext).HeroCategoryGrainClient;
					return await client.GetAll();
				})
				;
		}
	}
}
