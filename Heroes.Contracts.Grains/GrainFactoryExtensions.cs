using Orleans;

namespace Heroes.Contracts.Grains
{
	public static class GrainFactoryExtensions
	{
		public static IHeroGrain GetHeroGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroGrain>(key); 

		public static IHeroCollectionGrain GetHeroCollectionGrain(this IGrainFactory factory)
			=> factory.GetGrain<IHeroCollectionGrain>(0);
	}
}