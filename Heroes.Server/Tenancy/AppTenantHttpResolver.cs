using Sketch7.Multitenancy;
using Sketch7.Multitenancy.AspNet;

namespace Heroes.Server.Tenancy;

/// <summary>Resolves the current tenant from the <c>{tenant}</c> route segment or the <c>X-Tenant</c> header.</summary>
public sealed class AppTenantHttpResolver : ITenantHttpResolver<AppTenant>
{
	private readonly ITenantRegistry<AppTenant> _registry;

	public AppTenantHttpResolver(ITenantRegistry<AppTenant> registry)
	{
		_registry = registry;
	}

	public ValueTask<AppTenant?> Resolve(HttpContext httpContext)
	{
		// REST routes supply the tenant via the {tenant} route segment;
		// GraphQL and other non-routed callers pass it in the X-Tenant header.
		var tenantKey = httpContext.GetRouteValue("tenant") as string
			?? httpContext.Request.Headers["X-Tenant"].FirstOrDefault();

		if (string.IsNullOrEmpty(tenantKey))
			return ValueTask.FromResult<AppTenant?>(null);

		var tenant = _registry.GetAll().FirstOrDefault(t => t.Key == tenantKey);
		return ValueTask.FromResult(tenant);
	}
}
