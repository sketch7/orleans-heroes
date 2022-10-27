using System.Diagnostics;

namespace Heroes.Grains;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct TenantKeyData
{
	private string DebuggerDisplay => $"Tenant: '{Tenant}'";

	public static string Template = "tenant/{tenant}";

	public string Tenant { get; set; }
}