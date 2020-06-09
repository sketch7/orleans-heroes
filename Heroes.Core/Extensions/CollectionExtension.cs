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
	}
}