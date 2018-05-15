using System;
using System.Collections.Concurrent;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Heroes.Core
{
	public static class ReflectionExtensions
	{
		private static readonly ConcurrentDictionary<Type, string> DeymstifiedTypeNameCache = new ConcurrentDictionary<Type, string>();

		/// <summary>
		/// Gets the type name as a more clarified name when having generics e.g.
		/// returns "Odin.MultiGen`2[Odin.Hero,System.Collections.Generic.List`1[Odin.Heroic.Role]]" into "MultiGen&lt;Hero, List&lt;Role&gt;&gt;".
		/// </summary>
		/// <param name="type">Type to get demystified name for.</param>
		/// <returns></returns>
		public static string GetDemystifiedName(this Type type)
		{
			return DeymstifiedTypeNameCache.GetOrAdd(type, arg =>
			{
				if (type.GenericTypeArguments.Length == 0)
					return type.Name;

				var genericArgBuilder = new StringBuilder($"{type.Name.Remove(type.Name.Length - 2)}<");
				for (var index = 0; index < type.GenericTypeArguments.Length; index++)
				{
					var genericArg = type.GenericTypeArguments[index];
					var demystifiedGenericArgName = GetDemystifiedName(genericArg);
					genericArgBuilder.Append(demystifiedGenericArgName);
					if (type.GenericTypeArguments.Length - index > 1)
						genericArgBuilder.Append(", ");
				}

				genericArgBuilder.Append(">");
				return genericArgBuilder.ToString();
			});
		}
	}
}