namespace Heroes.Server.Gql;

public class AppSchema : Schema
{
	public AppSchema(IServiceProvider provider)
		: base(provider)
	{
		Query = provider.GetRequiredService<AppGraphQuery>();
		Subscription = provider.GetRequiredService<AppGraphSubscription>();
	}
}