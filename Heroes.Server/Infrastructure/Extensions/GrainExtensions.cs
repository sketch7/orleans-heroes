// Keep extension methods in the Orleans namespace so they are available
// anywhere Orleans grains are used without an additional using directive.
// ReSharper disable once CheckNamespace
namespace Orleans;

public static class GrainExtensions
{
	/// <summary>Returns the primary key of the grain of any type as a string.</summary>
	public static string GetPrimaryKeyAny(this Grain grain)
		=> grain.GetPrimaryKeyString()
			?? (grain.IsPrimaryKeyBasedOnLong()
				? grain.GetPrimaryKeyLong().ToString()
				: grain.GetPrimaryKey().ToString());
}
