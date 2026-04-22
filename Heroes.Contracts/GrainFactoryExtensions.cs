using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Contracts.Stats;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Contracts;

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


		// todo: these should be multi tenant
		public IHeroAbilitiesGrain GetHeroAbilitiesGrain(string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(key);

		public IHeroStatsGrain GetHeroStatsGrain(string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}