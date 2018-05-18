using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Heroes.Contracts.Grains;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.SiloHost.ConsoleApp
{
	public class TenantContainerBuilder
	{
		private readonly IExportRegistrationBlock _exportConfig;
		private readonly ITenantContext _tenant;

		public TenantContainerBuilder(IExportRegistrationBlock exportConfig, ITenantContext tenant)
		{
			_exportConfig = exportConfig;
			_tenant = tenant;
		}

		public void PopulateFrom(Action<IServiceCollection> configure, IServiceCollection services = null)
		{
			services = services ?? new ServiceCollection();
			configure(services);

			Register(_exportConfig, services, _tenant);
		}

		private static void Register(IExportRegistrationBlock c, IEnumerable<ServiceDescriptor> descriptors, ITenantContext tenant)
		{
			foreach (var descriptor in descriptors)
			{
				if ((object)descriptor.ImplementationType != null)
					c.Export(descriptor.ImplementationType).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);
				else if (descriptor.ImplementationFactory != null)
					c.ExportFactory(descriptor.ImplementationFactory).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);
				else
					c.ExportInstance(descriptor.ImplementationInstance).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);

				c.ExportPerTenantFactory(descriptor.ServiceType);
			}
		}
	}

	public static class GraceExtensions
	{
		// todo: ask Grace to expose?
		public static IFluentExportStrategyConfiguration ConfigureLifetime(this IFluentExportStrategyConfiguration configuration, ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Singleton:
					return configuration.Lifestyle.Singleton();
				case ServiceLifetime.Scoped:
					return configuration.Lifestyle.SingletonPerScope();
			}

			return configuration;
		}

		// todo: ask Grace to expose?
		public static IFluentExportInstanceConfiguration<T> ConfigureLifetime<T>(this IFluentExportInstanceConfiguration<T> configuration, ServiceLifetime lifecycleKind)
		{
			switch (lifecycleKind)
			{
				case ServiceLifetime.Singleton:
					return configuration.Lifestyle.Singleton();
				case ServiceLifetime.Scoped:
					return configuration.Lifestyle.SingletonPerScope();
			}

			return configuration;
		}

		public static TenantContainerBuilder ForTenant(this IExportRegistrationBlock config, ITenantContext tenant)
			=> new TenantContainerBuilder(config, tenant);

		public static IExportRegistrationBlock ExportPerTenantFactory<T>(this IExportRegistrationBlock config)
			=> config.ExportPerTenantFactory(typeof(T));

		public static IExportRegistrationBlock ExportPerTenantFactory(this IExportRegistrationBlock config, Type interfaceType)
		{
			config.ExportFactory<IExportLocatorScope, ITenantContext, object>((scope, tenantContext) =>
			{
				var tenant = tenantContext?.Key;

				if (tenant == null) throw new ArgumentNullException("tenant", "Tenant must be defined");
				return scope.Locate(withKey: tenant, type: interfaceType);
			}).As(interfaceType);
			return config;
		}

		public static IExportRegistrationBlock ExportForAllTenants<TInterface, TImplementation>(
			this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants,
			Action<IFluentExportStrategyConfiguration> configure = null)
			=> config.ExportForAllTenants(tenants, typeof(TInterface), typeof(TImplementation), configure);
		//public static IExportRegistrationBlock ExportForAllTenants<TInterface, TImplementation>(
		//	this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants,
		//	Action<IFluentExportStrategyConfiguration> configure = null)
		//{
		//	foreach (var tenant in tenants)
		//	{
		//		var exportConfig = config.Export<TImplementation>().AsKeyed<TInterface>(tenant.Key);
		//		//configure?.Invoke(exportConfig);
		//	}

		//	config.ExportPerTenantFactory<TInterface>();

		//	return config;
		//}

		public static IExportRegistrationBlock ExportForAllTenants(this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants, Type interfaceType, Type implementationType, Action<IFluentExportStrategyConfiguration> configure = null)
		{
			foreach (var tenant in tenants)
			{
				var exportConfig = config.Export(implementationType).AsKeyed(interfaceType, tenant.Key);
				configure?.Invoke(exportConfig);
			}

			config.ExportPerTenantFactory(interfaceType);

			return config;
		}
	}
}