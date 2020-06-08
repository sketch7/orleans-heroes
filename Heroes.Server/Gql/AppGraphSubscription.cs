using GraphQL.Types;
using Heroes.Server.Gql.Types;
using Heroes.Server.Sample;

namespace Heroes.Server.Gql
{
	public class AppGraphSubscription : ObjectGraphType
	{
		public AppGraphSubscription(
			IHeroService service
		)
		{
			FieldSubscribe<ListGraphType<HeroGraphType>>(
				name: "heroAdded",
				description: "hero added",
				resolve: context => service.Heroes(),
				subscribe: context => service.AddedHero()
			);
		}
	}
}