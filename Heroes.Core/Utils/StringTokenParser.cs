using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Heroes.Core.Utils
{
	// reference: https://gist.github.com/wcharczuk/2284226.
	/// <summary>
	/// String token parser which parses a template and extracts its data into named variables.
	/// </summary>
	/// <example>
	/// <code>
	/// var parser = new StringTokenParser("{protocol}://mydomain.com/{itemCategory}/{itemId}");
	/// var variables = parser.Variables; //should be .Count == 3
	/// var values = parser.Parse("https://mydomain.com/foo/1");
	/// //values = { { "protocol" => "https"}, { "itemCategory" => "foo"}, { "itemId" => "1" } }
	/// </code>
	/// </example>
	public class StringTokenParser
	{
		// the 0 and 1 are used by the string.format function, they are the start and end characters.
		private const string TemplateTokenPattern = @"[{0}].+?[{1}]";

		// the <>'s denote the group name; this is used for reference for the variables later.
		private const string VariableTokenPattern = "(?<{0}>[^,]*)";

		/// <summary>
		/// Gets the template pattern build from the template.
		/// </summary>
		private readonly Regex _templatePattern;

		public StringTokenParser(string template, char variableStartChar = '{', char variableEndChar = '}')
		{
			Template = template;
			VariableStartChar = variableStartChar;
			VariableEndChar = variableEndChar;
			_templatePattern = BuildTemplate();
		}

		/// <summary>
		/// Gets the template pattern that values are extracted from.
		/// </summary>
		/// <value>
		/// A string containing variables denoted by the <c>VariableStartChar</c> and the <c>VariableEndChar</c>
		/// </value>
		public string Template { get; }

		/// <summary>
		/// Gets the character that denotes the beginning of a variable name.
		/// </summary>
		public char VariableStartChar { get; }

		/// <summary>
		/// Gets the character that denotes the end of a variable name.
		/// </summary>
		public char VariableEndChar { get; }

		/// <summary>
		/// Gets the variable names parsed from the <c>Template</c>.
		/// </summary>
		public IReadOnlyCollection<string> Variables { get; private set; }

		/// <summary>
		/// Extract variable values from a given instance.
		/// </summary>
		/// <param name="value">Value to parse.</param>
		/// <returns>Dictionary with variable names mapped to values.</returns>
		public Dictionary<string, string> Parse(string value)
		{
			var inputValues = new Dictionary<string, string>();
			if (string.IsNullOrWhiteSpace(value))
				return inputValues;

			var matchCollection = _templatePattern.Match(value);

			foreach (var variable in Variables)
			{
				var varValue = matchCollection.Groups[variable].Value;
				inputValues.Add(variable, varValue);
			}

			return inputValues;
		}

		/// <summary>
		/// Extract variable values and map to class.
		/// </summary>
		/// <typeparam name="T">Class to cast result into.</typeparam>
		/// <param name="value">Value to parse.</param>
		/// <returns></returns>
		public T Parse<T>(string value) where T : new()
			=> Parse(value).ToObject<T, string>();

		/// <summary>
		/// Initialize the Variables set based on the <c>Template</c>
		/// </summary>
		private Regex BuildTemplate()
		{
			var matchCollection = Regex.Matches
			(
				Template,
				string.Format(TemplateTokenPattern, VariableStartChar, VariableEndChar),
				RegexOptions.IgnoreCase
			);

			var vars = new HashSet<string>();
			foreach (var match in matchCollection)
				vars.Add(RemoveVariableChars(match.ToString()));

			var templatePattern = Template;
			foreach (var variable in vars)
				templatePattern = templatePattern.Replace(WrapWithVariableChars(variable), string.Format(VariableTokenPattern, variable));
			var templateRegex = new Regex(templatePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

			Variables = vars;
			return templateRegex;
		}

		private string RemoveVariableChars(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			input = input.Replace(VariableStartChar.ToString(), string.Empty)
				.Replace(VariableEndChar.ToString(), string.Empty);
			return input;
		}

		private string WrapWithVariableChars(string input) => string.Format("{0}{1}{2}", VariableStartChar, input, VariableEndChar);
	}
}