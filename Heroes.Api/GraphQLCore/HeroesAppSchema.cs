using GraphQL.Types;
using Heroes.Api.GraphQLCore.Queries;

namespace Heroes.Api.GraphQLCore
{
	public class HeroesAppSchema : Schema
	{
		public HeroesAppSchema(
			HeroesAppGraphQuery query,
			HeroesAppGraphSubscription subscription
		)
		{
			Query = query;
			Subscription = subscription;
		}
	}
}