using System.Collections.Generic;
using System.Diagnostics;

namespace Heroes.Contracts.Grains
{
	public interface ITenantContext
	{
		string Key { get; set; }
		string Name { get; set; }
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class TenantContext : ITenantContext
	{
		private string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}'";

		public string Key { get; set; }
		public string Name { get; set; }
	}

	public static class Tenants
	{
		public static TenantContext LeageOfLegends = new TenantContext
		{
			Key = "lol",
			Name = "League of Legends"
		};

		public static TenantContext HeroesOfTheStorm = new TenantContext
		{
			Key = "hots",
			Name = "Heroes of the Storm"
		};

		public static HashSet<TenantContext> All = new HashSet<TenantContext>
		{
			LeageOfLegends,
			HeroesOfTheStorm
		};
	}
}