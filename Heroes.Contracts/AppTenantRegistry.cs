using Heroes.Core.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Heroes.Contracts
{
	public interface IAppTenantRegistry : ITenantRegistry<AppTenant>
	{
		ITenant GetByPrimaryKey(string primaryKey);
	}

	public class AppTenantRegistry : IAppTenantRegistry
	{
		private const string TenantGroupKey = "tenant";
		private readonly Regex _regex = new Regex($@"tenant\/(?'{TenantGroupKey}'[\w@-]+)\/?(.*)", RegexOptions.Compiled);

		public static AppTenant LeagueOfLegends = new AppTenant
		{
			Key = "lol",
			Name = "League of Legends"
		};

		public static readonly AppTenant HeroesOfTheStorm = new AppTenant
		{
			Key = "hots",
			Name = "Heroes of the Storm"
		};

		private readonly Dictionary<string, AppTenant> _tenants;

		public AppTenantRegistry()
		{
			var tenants = new HashSet<AppTenant>
			{
				LeagueOfLegends,
				HeroesOfTheStorm
			};

			_tenants = tenants.ToDictionary(x => x.Key);
		}

		public AppTenant Get(string tenant)
		{
			var config = GetOrDefault(tenant);
			if (config == null)
				throw new KeyNotFoundException($"Brand not found for brand: '{tenant}'");
			return config;
		}

		public AppTenant GetOrDefault(string tenant)
		{
			_tenants.TryGetValue(tenant, out var brand);
			return brand;
		}

		public ITenant GetByPrimaryKey(string primaryKey)
		{
			if (string.IsNullOrEmpty(primaryKey))
				throw new ArgumentException("Primary key must be defined.", nameof(primaryKey));

			var matches = _regex.Match(primaryKey);
			if (!matches.Success)
				return null;

			var tenantKey = matches.Groups[TenantGroupKey].Value;
			return Get(tenantKey);
		}

		public IEnumerable<AppTenant> GetAll() => _tenants.Values;
	}
}