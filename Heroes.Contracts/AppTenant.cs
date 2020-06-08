using Heroes.Core.Tenancy;
using System.Diagnostics;

namespace Heroes.Contracts
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class AppTenant : ITenant
	{
		private string DebuggerDisplay => $"Key: '{Key}', Name: '{Name}'";

		public string Key { get; set; }
		public string Name { get; set; }
	}
}