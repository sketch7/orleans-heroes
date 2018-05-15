using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Heroes.Core
{
	public static class CollectionExtension
	{
		private static readonly Random Random = new Random();

		public static T RandomElement<T>(this IList<T> list)
		{
			return list[Random.Next(list.Count)];
		}

		public static T RandomElement<T>(this T[] array)
		{
			return array[Random.Next(array.Length)];
		}
	}
}