using System.Diagnostics;

namespace Heroes.Grains;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct TenantKeyData
{
private string DebuggerDisplay => $"Tenant: '{Tenant}', GrainKey: '{GrainKey}'";

public static string Template = "tenant/{tenant}/{grainKey}";

public string Tenant { get; set; }
public string GrainKey { get; set; }
}
