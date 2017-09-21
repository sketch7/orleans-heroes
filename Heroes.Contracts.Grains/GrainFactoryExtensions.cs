using Orleans;

namespace Heroes.Contracts.Grains
{
	public static class GrainFactoryExtensions
	{
		public static IHeroGrain GetHeroGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroGrain>(key);

		public static IHeroCollectionGrain GetHeroCollectionGrain(this IGrainFactory factory)
			=> factory.GetGrain<IHeroCollectionGrain>(0);

		public static IHeroAbilitiesGrain GetHeroAbilitiesGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(key);

		public static IHeroStatsGrain GetHeroStatsGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}