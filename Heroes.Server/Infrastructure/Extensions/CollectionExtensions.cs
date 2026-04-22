using Humanizer;

namespace Heroes.Server.Infrastructure;

public static class CollectionExtensions
{
	extension<T>(IEnumerable<T> collection)
	{
		public Task<TResult[]> SelectAsync<TResult>(Func<T, Task<TResult>> transform)
			=> Task.WhenAll(collection.Select(transform));
	}

	extension<TResult, TValue>(IDictionary<string, TValue> source) where TResult : new()
	{
		/// <summary>Converts a dictionary to a generic type.</summary>
		/// <param name="throwIfNotFound">Determine whether to throw an exception when property not found or not.</param>
		/// <param name="keyTransform">Key transform function to match property name (default: pascalize).</param>
		public TResult ToObject(
			bool throwIfNotFound = false,
			Func<string, string>? keyTransform = null
		)
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
}
