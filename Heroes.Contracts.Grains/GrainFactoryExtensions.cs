using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Stats;
using Orleans;

namespace Heroes.Contracts.Grains
{
	public static class GrainFactoryExtensions
	{
		public static IHeroGrain GetHeroGrain(this IGrainFactory factory, string tenant, string key)
			=> factory.GetGrain<IHeroGrain>($"tenant/{tenant}/{key}");

		public static IHeroCollectionGrain GetHeroCollectionGrain(this IGrainFactory factory, string tenant)
			=> factory.GetGrain<IHeroCollectionGrain>($"tenant/{tenant}");

		// todo: these should be multi tenant
		public static IHeroAbilitiesGrain GetHeroAbilitiesGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(key);

		// todo: these should be multi tenant
		public static IHeroStatsGrain GetHeroStatsGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}