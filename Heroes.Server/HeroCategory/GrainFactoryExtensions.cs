using Sketch7.Multitenancy.Orleans;

#pragma warning disable IDE0130 // Namespace intentionally matches extended type not folder
namespace Heroes.Server;

public static class HeroCategoryGrainFactoryExtensions
{
	extension(IGrainFactory factory)
	{
		public IHeroCategoryGrain GetHeroCategoryGrain(string tenant, string key)
			=> factory.GetGrain<IHeroCategoryGrain>(TenantGrainKey.Create(tenant, key));

		public IHeroCategoryCollectionGrain GetHeroCategoryCollectionGrain(string tenant)
			=> factory.GetGrain<IHeroCategoryCollectionGrain>(TenantGrainKey.Create(tenant));
	}
}
