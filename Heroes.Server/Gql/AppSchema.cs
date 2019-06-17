using GraphQL.Types;

namespace Heroes.Server.Gql
{
	public class AppSchema : Schema
	{
		public AppSchema(
			AppGraphQuery query,
			AppGraphSubscription subscription
		)
		{
			Query = query;
			Subscription = subscription;
		}
	}
}