using Humanizer;

namespace Heroes.Server.Infrastructure;

public static class CollectionExtensions
{
	private static readonly Random Random = new();

	public static T RandomElement<T>(this IList<T> list) => list[Random.Next(list.Count)];

	public static T RandomElement<T>(this T[] array) => array[Random.Next(array.Length)];

	extension<T>(IEnumerable<T> collection)
	{
		public Task ForEachAsync(Func<T, Task> transform)
			=> Task.WhenAll(collection.Select(transform));

		public Task<TResult[]> SelectAsync<TResult>(Func<T, Task<TResult>> transform)
			=> Task.WhenAll(collection.Select(transform));
	}

	/// <summary>Converts a dictionary to a generic type.</summary>
	/// <typeparam name="TValue">Dictionary value type.</typeparam>
	/// <typeparam name="TResult">Generic type to convert dictionary to.</typeparam>
	/// <param name="source">Data source to populate object with.</param>
	/// <param name="throwIfNotFound">Determine whether to throw an exception when property not found or not.</param>
	/// <param name="keyTransform">Key transform function to match property name (default: pascalize).</param>
	public static TResult ToObject<TResult, TValue>(
		this IDictionary<string, TValue> source,
		bool throwIfNotFound = false,
		Func<string, string>? keyTransform = null
	) where TResult : new()
	{
		object obj = new TResult();
		var objType = obj.GetType();
		keyTransform ??= InflectorExtensions.Pascalize;

		foreach (var item in source)
		{
			var propName = keyTransform(item.Key);
			var prop = objType.GetProperty(propName);
			if (prop == null)
			{
				if (throwIfNotFound)
					throw new MissingMemberException(objType.GetDemystifiedName(), item.Key);
				continue;
			}

			if (prop.PropertyType.IsEnum && item.Value is string strValue)
				prop.SetValue(obj, Enum.Parse(prop.PropertyType, strValue, ignoreCase: true), null);
			else
				prop.SetValue(obj, item.Value, null);
		}

		return (TResult)obj;
	}
}
