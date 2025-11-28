using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Heroes.Contracts;
using Heroes.Core.Tenancy;
using Orleans;
using Orleans.Runtime;
using System.Text.RegularExpressions;

namespace Heroes.Server.Infrastructure;

/// <summary>
/// <see cref="IGrainActivator"/> that uses type activation to create grains.
/// </summary>
public class TenantGrainActivator : IGrainActivator
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IAppTenantRegistry _appTenantRegistry;
	private readonly ILogger<TenantGrainActivator> _logger;
	private const string TenantGroupKey = "brand";
	private readonly Regex _regex = new Regex($@"tenant\/(?'{TenantGroupKey}'[\w@-]+)\/?(.*)", RegexOptions.Compiled);

	public TenantGrainActivator(
		IServiceProvider serviceProvider,
		IAppTenantRegistry appTenantRegistry,
		ILogger<TenantGrainActivator> logger
	)
	{
		_serviceProvider = serviceProvider;
		_appTenantRegistry = appTenantRegistry;
		_logger = logger;
	}

	public object CreateInstance(IGrainContext context)
	{
		var grainType = context.GrainInstance?.GetType() ?? typeof(object);

		if (context.GrainId.Key.ToString() == null)
			try
			{
				return ActivatorUtilities.CreateInstance(_serviceProvider, grainType, context);
			}
			catch (LocateException ex)
			{
				var type = grainType.GetDemystifiedName();
				_logger.LogError(ex, "Grain does not have a tenant grain primary key. PrimaryKey: {grainPrimaryKey}, grain: {grain}",
					context.GrainId.Key,
					type
				);
				throw;
			}

		var scope = context.ActivationServices.GetRequiredService<IExportLocatorScope>();
		var tenant = _appTenantRegistry.GetByPrimaryKey(context.GrainId.Key.ToString());        //if (tenant == null && Const.OwnedNamespaces.Any(context.GrainType.Namespace.Contains))
																								//{
																								//	if (!SharedTenantGrains.TryGetValue(context.GrainType, out var ignoreTenantWarning))
																								//	{
																								//		var sharedTenant = context.GrainType.GetAttribute<SharedTenantAttribute>();
																								//		ignoreTenantWarning = sharedTenant != null;
																								//		SharedTenantGrains[context.GrainType] = ignoreTenantWarning;
																								//	}

		//	if (!ignoreTenantWarning)
		//	{
		//		var type = context.GrainType.GetDemystifiedName();
		//		_logger.LogWarn("No tenant resolved for grain. PrimaryKey: {grainPrimaryKey}, grain: {grain}",
		//			context.GrainIdentity.PrimaryKeyString,
		//			type
		//		);
		//	}
		//}

		scope.SetTenantContext(tenant);

		return ActivatorUtilities.CreateInstance(_serviceProvider, grainType, context);
	}

	public ValueTask DisposeInstance(IGrainContext context, object grain)
	{
		if (grain is IDisposable disposable)
		{
			disposable.Dispose();
		}
		return ValueTask.CompletedTask;
	}
}