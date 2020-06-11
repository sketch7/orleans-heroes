using System.Collections.Concurrent;

namespace Heroes.Core.Utils
{
	public interface IStringTokenParserFactory
	{
		/// <summary>
		/// Get or create and cache the string token parser for the specified template.
		/// </summary>
		/// <param name="template">Template to get parser for.</param>
		/// <returns></returns>
		StringTokenParser Get(string template);
	}

	public class StringTokenParserFactory : IStringTokenParserFactory
	{
		private static readonly ConcurrentDictionary<string, StringTokenParser> TemplateParserCache
			= new ConcurrentDictionary<string, StringTokenParser>();

		/// <inheritdoc />
		public StringTokenParser Get(string template)
			=> TemplateParserCache.GetOrAdd(template, arg => new StringTokenParser(arg));
	}
}
