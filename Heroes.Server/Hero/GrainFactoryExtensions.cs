using Sketch7.Multitenancy.Orleans;

#pragma warning disable IDE0130 // Namespace intentionally matches extended type not folder
namespace Heroes.Server;

public static class HeroGrainFactoryExtensions
{
	extension(IGrainFactory factory)
	{
		public IHeroGrain GetHeroGrain(string tenant, string key)
			=> factory.GetGrain<IHeroGrain>(TenantGrainKey.Create(tenant, key));

		public IHeroCollectionGrain GetHeroCollectionGrain(string tenant)
			=> factory.GetGrain<IHeroCollectionGrain>(TenantGrainKey.Create(tenant));

		public IHeroAbilitiesGrain GetHeroAbilitiesGrain(string tenant, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(TenantGrainKey.Create(tenant, key));
	}
}
