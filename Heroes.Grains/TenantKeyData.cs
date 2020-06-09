using System.Diagnostics;

namespace Heroes.Grains
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public struct TenantKeyData
	{
		private string DebuggerDisplay => $"Tenant: '{Tenant}'";

		public string Tenant { get; set; }
	}
}