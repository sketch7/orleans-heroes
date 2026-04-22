namespace Heroes.Server.Infrastructure;

public static class CollectionExtensions
{
	extension<T>(IEnumerable<T> collection)
	{
		public Task<TResult[]> SelectAsync<TResult>(Func<T, Task<TResult>> transform)
			=> Task.WhenAll(collection.Select(transform));
	}
}
