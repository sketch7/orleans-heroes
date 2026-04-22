namespace Heroes.Server.Gql;

public sealed class AppSchema : Schema
{
	public AppSchema(IServiceProvider provider)
		: base(provider)
	{
		Query = provider.GetRequiredService<AppGraphQuery>();
	}
}