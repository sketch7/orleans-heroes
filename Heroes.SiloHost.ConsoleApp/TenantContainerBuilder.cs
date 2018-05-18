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
		private readonly ITenant _tenant;

		public TenantContainerBuilder(IExportRegistrationBlock exportConfig, ITenant tenant)
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

		private static void Register(IExportRegistrationBlock c, IEnumerable<ServiceDescriptor> descriptors, ITenant tenant)
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

	/// <summary>
	/// Tenants config builder per Tenant.
	/// </summary>
	/// <typeparam name="TTenant"></typeparam>
	public class TenantsContainerBuilder<TTenant>
		where TTenant : class, ITenant
	{
		private readonly List<TenantContainerBuilderConfigItem<TTenant>> _tenantsConfigs = new List<TenantContainerBuilderConfigItem<TTenant>>();

		public List<TenantContainerBuilderConfigItem<TTenant>> GetAll()
			=> _tenantsConfigs;

		public TenantsContainerBuilder<TTenant> ForTenant(string key, Action<TenantContainerBuilder> configure)
			=> ForTenant(x => x.Key == key, configure);

		public TenantsContainerBuilder<TTenant> ForTenant(Func<TTenant, bool> predicate, Action<TenantContainerBuilder> configure)
		{
			_tenantsConfigs.Add(new TenantContainerBuilderConfigItem<TTenant>
			{
				TenantFilter = predicate,
				Configure = configure
			});

			return this;
		}
	}

	public struct TenantContainerBuilderConfigItem<TTenant>
		where TTenant : class, ITenant
	{
		public Func<TTenant, bool> TenantFilter { get; set; }
		public Action<TenantContainerBuilder> Configure { get; set; }
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

		public static TenantContainerBuilder ForTenant(this IExportRegistrationBlock config, ITenant tenant)
			=> new TenantContainerBuilder(config, tenant);

		/// <summary>
		/// Export a specific type that requires a tenant.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IExportRegistrationBlock ExportPerTenantFactory<TInterface>(this IExportRegistrationBlock config)
			=> config.ExportPerTenantFactory(typeof(TInterface));

		public static IExportRegistrationBlock ExportPerTenantFactory(this IExportRegistrationBlock config, Type interfaceType)
		{
			config.ExportFactory<IExportLocatorScope, ITenant, object>((scope, tenantContext) =>
			{
				var tenant = tenantContext?.Key;

				if (tenant == null) throw new ArgumentNullException("tenant", "AppTenant must be defined");
				return scope.Locate(withKey: tenant, type: interfaceType);
			}).As(interfaceType);
			return config;
		}

		/// <summary>
		/// Export a service/interface for all Tenants individually.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <typeparam name="TImplementation"></typeparam>
		/// <param name="config"></param>
		/// <param name="tenants"></param>
		/// <param name="configure"></param>
		/// <returns></returns>
		public static IExportRegistrationBlock ExportForAllTenants<TInterface, TImplementation>(
			this IExportRegistrationBlock config, IEnumerable<ITenant> tenants,
			Action<IFluentExportStrategyConfiguration> configure = null)
			=> config.ExportForAllTenants(tenants, typeof(TInterface), typeof(TImplementation), configure);

		public static IExportRegistrationBlock ExportForAllTenants(this IExportRegistrationBlock config, IEnumerable<ITenant> tenants, Type interfaceType, Type implementationType, Action<IFluentExportStrategyConfiguration> configure = null)
		{
			foreach (var tenant in tenants)
			{
				var exportConfig = config.Export(implementationType).AsKeyed(interfaceType, tenant.Key);
				configure?.Invoke(exportConfig);
			}

			config.ExportPerTenantFactory(interfaceType);

			return config;
		}

		/// <summary>
		/// Create and configure a new registration block for tenants container configuration.
		/// </summary>
		/// <typeparam name="TTenant">Tenant Type.</typeparam>
		/// <param name="config"></param>
		/// <param name="tenants"></param>
		/// <param name="configure"></param>
		/// <returns></returns>
		public static IExportRegistrationBlock ForTenants<TTenant>(this IExportRegistrationBlock config,
			IEnumerable<TTenant> tenants,
			Action<TenantsContainerBuilder<TTenant>> configure
		) where TTenant : class, ITenant
		{
			var tenantsContainerBuilder = new TenantsContainerBuilder<TTenant>();
			configure(tenantsContainerBuilder);

			var configs = tenantsContainerBuilder.GetAll();

			foreach (var tenant in tenants)
			{
				foreach (var tenantConfig in configs)
				{
					if (!tenantConfig.TenantFilter(tenant))
						continue;

					tenantConfig.Configure(new TenantContainerBuilder(config, tenant));
				}
			}

			return config;
		}
	}
}