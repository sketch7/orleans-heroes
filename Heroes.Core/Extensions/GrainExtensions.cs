// ReSharper disable once CheckNamespace
namespace Orleans
{
	public static class GrainExtensions
	{
		/// <summary>
		/// Returns the primary key of the grain of any type as a string.
		/// </summary>
		/// <param name="grain"></param>
		/// <returns></returns>
		public static string GetPrimaryKeyAny(this Grain grain) //  todo: ideally this should be IGrain instead, if Grain implemented IGrain.
		{
			return grain.GetPrimaryKeyString()
				   ?? (grain.IsPrimaryKeyBasedOnLong()
					   ? grain.GetPrimaryKeyLong().ToString()
					   : grain.GetPrimaryKey().ToString());
		}
	}
}
