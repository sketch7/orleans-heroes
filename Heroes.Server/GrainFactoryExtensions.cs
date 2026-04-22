using Heroes.Server.HeroCategory;
using Heroes.Server.Hero;
using Heroes.Server.HeroStat;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server;

public static class GrainFactoryExtensions
{
	extension(IGrainFactory factory)
	{
		public IHeroGrain GetHeroGrain(string tenant, string key)
			=> factory.GetGrain<IHeroGrain>(TenantGrainKey.Create(tenant, key));

		public IHeroCollectionGrain GetHeroCollectionGrain(string tenant)
			=> factory.GetGrain<IHeroCollectionGrain>(TenantGrainKey.Create(tenant));

		public IHeroCategoryGrain GetHeroCategoryGrain(string tenant, string key)
			=> factory.GetGrain<IHeroCategoryGrain>(TenantGrainKey.Create(tenant, key));

		public IHeroCategoryCollectionGrain GetHeroCategoryCollectionGrain(string tenant)
			=> factory.GetGrain<IHeroCategoryCollectionGrain>(TenantGrainKey.Create(tenant));

		public IHeroAbilitiesGrain GetHeroAbilitiesGrain(string tenant, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(TenantGrainKey.Create(tenant, key));

		// todo: make multi-tenant
		public IHeroStatsGrain GetHeroStatsGrain(string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}
