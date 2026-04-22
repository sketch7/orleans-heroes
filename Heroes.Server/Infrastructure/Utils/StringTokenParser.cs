using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Heroes.Server.Infrastructure.Utils;

// reference: https://gist.github.com/wcharczuk/2284226
/// <summary>
/// Parses a template string and extracts named variable values.
/// </summary>
/// <example>
/// <code>
/// var parser = new StringTokenParser("{protocol}://mydomain.com/{itemCategory}/{itemId}");
/// var values = parser.Parse("https://mydomain.com/foo/1");
/// // values = { "protocol" => "https", "itemCategory" => "foo", "itemId" => "1" }
/// </code>
/// </example>
public sealed class StringTokenParser
{
	private const string TemplateTokenPattern = @"[{0}].+?[{1}]";
	private const string VariableTokenPattern = "(?<{0}>[^,]*)";

	private readonly Regex _templatePattern;

	public StringTokenParser(string template, char variableStartChar = '{', char variableEndChar = '}')
	{
		Template = template;
		VariableStartChar = variableStartChar;
		VariableEndChar = variableEndChar;
		_templatePattern = BuildTemplate();
	}

	public string Template { get; }
	public char VariableStartChar { get; }
	public char VariableEndChar { get; }
	public IReadOnlyCollection<string> Variables { get; private set; }

	/// <summary>Extract variable values from a given instance string.</summary>
	public Dictionary<string, string> Parse(string value)
	{
		var inputValues = new Dictionary<string, string>();
		if (string.IsNullOrWhiteSpace(value))
			return inputValues;

		var match = _templatePattern.Match(value);
		foreach (var variable in Variables)
			inputValues.Add(variable, match.Groups[variable].Value);

		return inputValues;
	}

	/// <summary>Extract variable values and map to class.</summary>
	public T Parse<T>(string value) where T : new()
		=> Parse(value).ToObject<T, string>();

	private Regex BuildTemplate()
	{
		var matchCollection = Regex.Matches(
			Template,
			string.Format(TemplateTokenPattern, VariableStartChar, VariableEndChar),
			RegexOptions.IgnoreCase
		);

		var vars = new HashSet<string>();
		foreach (var match in matchCollection)
			vars.Add(RemoveVariableChars(match.ToString()));

		var pattern = Template;
		foreach (var variable in vars)
			pattern = pattern.Replace(WrapWithVariableChars(variable), string.Format(VariableTokenPattern, variable));

		Variables = vars;
		return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}

	private string RemoveVariableChars(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return input;

		return input
			.Replace(VariableStartChar.ToString(), string.Empty)
			.Replace(VariableEndChar.ToString(), string.Empty);
	}

	private string WrapWithVariableChars(string input)
		=> $"{VariableStartChar}{input}{VariableEndChar}";
}

public interface IStringTokenParserFactory
{
	/// <summary>Get or create (and cache) the parser for the specified template.</summary>
	StringTokenParser Get(string template);
}

public sealed class StringTokenParserFactory : IStringTokenParserFactory
{
	private static readonly ConcurrentDictionary<string, StringTokenParser> Cache = new();

	/// <inheritdoc />
	public StringTokenParser Get(string template)
		=> Cache.GetOrAdd(template, t => new StringTokenParser(t));
}
