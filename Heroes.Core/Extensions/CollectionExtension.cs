using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Heroes.Core
{
	public static class CollectionExtension
	{
		private static readonly Random Random = new Random();

		public static T RandomElement<T>(this IList<T> list) => list[Random.Next(list.Count)];

		public static T RandomElement<T>(this T[] array) => array[Random.Next(array.Length)];

		public static Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> transform)
			=> Task.WhenAll(collection.Select(transform));

		public static Task<TResult[]> SelectAsync<TInput, TResult>(this IEnumerable<TInput> collection, Func<TInput, Task<TResult>> transform)
			=> Task.WhenAll(collection.Select(transform));

		/// <summary>
		/// Converts dictionary to generic type.
		/// </summary>
		/// <typeparam name="TValue">Dictionary value type.</typeparam>
		/// <typeparam name="TResult">Generic type to convert dictionary to.</typeparam>
		/// <param name="source">Data source to populate object with.</param>
		/// <param name="throwIfNotFound">Determine whether to throw an exception when property not found or not.</param>
		/// <param name="keyTransform">Key transform function to match property name (defaults: to pascalize).</param>
		/// <returns></returns>
		public static TResult ToObject<TResult, TValue>(this IDictionary<string, TValue> source,
			bool throwIfNotFound = false,
			Func<string, string> keyTransform = null
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
				{
					var enumValue = Enum.Parse(prop.PropertyType, strValue, ignoreCase: true);
					prop.SetValue(obj, enumValue, null);
				}
				else
					prop.SetValue(obj, item.Value, null);
			}

			return (TResult)obj;
		}

		/// <summary>
		/// Converts dictionary to generic type.
		/// </summary>
		/// <typeparam name="T">Generic type to convert dictionary to.</typeparam>
		/// <param name="source">Data source to populate object with.</param>
		/// <param name="throwIfNotFound">Determine whether to throw an exception when property not found or not.</param>
		/// <param name="keyTransform">Key transform function to match property name (defaults: to pascalize).</param>
		/// <returns></returns>
		public static T ToObject<T>(this IDictionary<string, string> source,
			bool throwIfNotFound = true,
			Func<string, string> keyTransform = null
		) where T : new()
			=> ToObject<T, string>(source, throwIfNotFound, keyTransform);

		/// <summary>
		/// Converts dictionary to generic type.
		/// </summary>
		/// <typeparam name="T">Generic type to convert dictionary to.</typeparam>
		/// <param name="source">Data source to populate object with.</param>
		/// <param name="throwIfNotFound">Determine whether to throw an exception when property not found or not.</param>
		/// <param name="keyTransform">Key transform function to match property name (defaults: to pascalize).</param>
		/// <returns></returns>
		public static T ToObject<T>(this IDictionary<string, object> source,
			bool throwIfNotFound = true,
			Func<string, string> keyTransform = null
		) where T : new()
			=> ToObject<T, object>(source, throwIfNotFound, keyTransform);
	}
}