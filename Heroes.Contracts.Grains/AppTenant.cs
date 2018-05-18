using System.Collections.Generic;
using System.Diagnostics;

namespace Heroes.Contracts.Grains
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class AppTenant : ITenant
	{
		private string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}'";

		public string Key { get; set; }
		public string Name { get; set; }
	}

	public static class Tenants
	{
		public static AppTenant LeageOfLegends = new AppTenant
		{
			Key = "lol",
			Name = "League of Legends"
		};

		public static AppTenant HeroesOfTheStorm = new AppTenant
		{
			Key = "hots",
			Name = "Heroes of the Storm"
		};

		public static HashSet<AppTenant> All = new HashSet<AppTenant>
		{
			LeageOfLegends,
			HeroesOfTheStorm
		};
	}
}