using Heroes.Contracts;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.AspNet;

namespace Heroes.Server.Tenancy;

/// <summary>Resolves the current tenant from the <c>{tenant}</c> route segment.</summary>
public sealed class AppTenantHttpResolver : ITenantHttpResolver<AppTenant>
{
	private readonly ITenantRegistry<AppTenant> _registry;

	public AppTenantHttpResolver(ITenantRegistry<AppTenant> registry)
	{
		_registry = registry;
	}

	public ValueTask<AppTenant?> Resolve(HttpContext httpContext)
	{
		var tenantKey = httpContext.GetRouteValue("tenant") as string;
		if (string.IsNullOrEmpty(tenantKey))
			return ValueTask.FromResult<AppTenant?>(null);

		var tenant = _registry.GetAll().FirstOrDefault(t => t.Key == tenantKey);
		return ValueTask.FromResult(tenant);
	}
}
