using GraphQL.Types;
using Heroes.Api.GraphQLCore.Types;
using Heroes.Api.Sample;

namespace Heroes.Api.GraphQLCore
{
	public class HeroesAppGraphSubscription : ObjectGraphType
	{
		public HeroesAppGraphSubscription(
				IHeroService service
			)
		{
			FieldSubscribe<ListGraphType<HeroType>>(
				name: "heroAdded",
				description: "hero added",
				resolve: context => service.Heroes(),
				subscribe: context => service.AddedHero()
			);
		}
	}
}
