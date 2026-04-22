using Sketch7.Multitenancy;

namespace Heroes.Contracts;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record AppTenant : ITenant
{
	private string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}'";

	public required string Key { get; init; }
	public required string Name { get; init; }
}