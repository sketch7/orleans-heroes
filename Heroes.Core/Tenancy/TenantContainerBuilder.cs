using Grace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Heroes.Core.Tenancy
{
	public class TenantContainerBuilder<TTenant>
		where TTenant : ITenant
	{
		private readonly IExportRegistrationBlock _exportConfig;
		private readonly TTenant _tenant;

		public TenantContainerBuilder(IExportRegistrationBlock exportConfig, TTenant tenant)
		{
			_exportConfig = exportConfig;
			_tenant = tenant;
		}

		public void PopulateFrom(Action<IServiceCollection> configure, IServiceCollection services = null, bool asPerTenant = true)
		{
			services = services ?? new ServiceCollection();
			configure(services);

			if (asPerTenant)
				RegisterPerTenant(_exportConfig, services, _tenant);
			else
				_exportConfig.PopulateFrom(services);
		}

		public void PopulateFrom(Action<IServiceCollection, TTenant> configure, IServiceCollection services = null, bool asPerTenant = true)
			=> PopulateFrom(s => configure(s, _tenant), services, asPerTenant);

		private static void RegisterPerTenant(IExportRegistrationBlock exportConfig, IEnumerable<ServiceDescriptor> descriptors, TTenant tenant)
		{
			foreach (var descriptor in descriptors)
			{
				if ((object)descriptor.ImplementationType != null)
					exportConfig.Export(descriptor.ImplementationType).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);
				else if (descriptor.ImplementationFactory != null)
					exportConfig.ExportFactory(descriptor.ImplementationFactory).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);
				else
					exportConfig.ExportInstance(descriptor.ImplementationInstance).AsKeyed(descriptor.ServiceType, tenant.Key).ConfigureLifetime(descriptor.Lifetime);

				exportConfig.ExportPerTenantFactory(descriptor.ServiceType);
			}
		}
	}

	/// <summary>
	/// Tenants config builder per Tenant.
	/// </summary>
	/// <typeparam name="TTenant"></typeparam>
	public class TenantsContainerBuilder<TTenant>
		where TTenant : ITenant
	{
		private readonly List<TenantContainerBuilderConfigItem<TTenant>> _tenantsConfigs = new List<TenantContainerBuilderConfigItem<TTenant>>();

		public List<TenantContainerBuilderConfigItem<TTenant>> GetAll()
			=> _tenantsConfigs;

		public TenantsContainerBuilder<TTenant> ForTenant(string key, Action<TenantContainerBuilder<TTenant>> configure)
			=> ForTenant(x => x.Key == key, configure);

		public TenantsContainerBuilder<TTenant> ForAll(Action<TenantContainerBuilder<TTenant>> configure)
			=> ForTenant((Func<TTenant, bool>)null, configure);

		public TenantsContainerBuilder<TTenant> ForTenant(Func<TTenant, bool> predicate, Action<TenantContainerBuilder<TTenant>> configure)
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
		where TTenant : ITenant
	{
		public Func<TTenant, bool> TenantFilter { get; set; }
		public Action<TenantContainerBuilder<TTenant>> Configure { get; set; }
	}
}
