using System.Collections.Concurrent;
using System.Text;

namespace Heroes.Server.Infrastructure;

public static class ReflectionExtensions
{
	private static readonly ConcurrentDictionary<Type, string> DemystifiedTypeNameCache = new();

	/// <summary>
	/// Gets the type name as a human-readable string, resolving generic type arguments.
	/// e.g. "MultiGen`2[Hero,List`1[Role]]" → "MultiGen&lt;Hero, List&lt;Role&gt;&gt;".
	/// </summary>
	public static string GetDemystifiedName(this Type type)
		=> DemystifiedTypeNameCache.GetOrAdd(type, t =>
		{
			if (t.GenericTypeArguments.Length == 0)
				return t.Name;

			var sb = new StringBuilder($"{t.Name.Remove(t.Name.Length - 2)}<");
			for (var i = 0; i < t.GenericTypeArguments.Length; i++)
			{
				sb.Append(GetDemystifiedName(t.GenericTypeArguments[i]));
				if (t.GenericTypeArguments.Length - i > 1)
					sb.Append(", ");
			}

			sb.Append('>');
			return sb.ToString();
		});
}
