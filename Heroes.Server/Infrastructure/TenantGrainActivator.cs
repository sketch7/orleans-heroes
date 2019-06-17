using System;
using System.Text.RegularExpressions;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Heroes.Contracts.Grains;
using Heroes.Core;
using Heroes.Core.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Heroes.Server.Infrastructure
{
	/// <summary>
	/// <see cref="IGrainActivator"/> that uses type activation to create grains.
	/// </summary>
	public class TenantGrainActivator : DefaultGrainActivator
	{
		private readonly IAppTenantRegistry _appTenantRegistry;
		private readonly ILogger<TenantGrainActivator> _logger;
		private const string TenantGroupKey = "brand";
		private readonly Regex _regex = new Regex($@"tenant\/(?'{TenantGroupKey}'[\w@-]+)\/?(.*)", RegexOptions.Compiled);

		public TenantGrainActivator(
			IServiceProvider serviceProvider,
			IAppTenantRegistry appTenantRegistry,
			ILogger<TenantGrainActivator> logger
		) : base(serviceProvider)
		{
			_appTenantRegistry = appTenantRegistry;
			_logger = logger;
		}

		public override object Create(IGrainActivationContext context)
		{
			if (context.GrainIdentity.PrimaryKeyString == null)
				try
				{
					return base.Create(context);
				}
				catch (LocateException ex)
				{
					var type = context.GrainType.GetDemystifiedName();
					_logger.LogError(ex, "Grain does not have a tenant grain primary key. PrimaryKey: {grainPrimaryKey}, grain: {grain}",
						context.GrainInstance?.GetPrimaryKeyAny(),
						type
					);
					throw;
				}

			var scope = context.ActivationServices.GetRequiredService<IExportLocatorScope>();
			var tenant = _appTenantRegistry.GetByPrimaryKey(context.GrainIdentity.PrimaryKeyString);

			//if (tenant == null && Const.OwnedNamespaces.Any(context.GrainType.Namespace.Contains))
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

			return base.Create(context);
		}
	}
}