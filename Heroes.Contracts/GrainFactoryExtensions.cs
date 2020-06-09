using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Contracts.Stats;
using Orleans;

namespace Heroes.Contracts
{
	public static class GrainFactoryExtensions
	{
		public static IHeroGrain GetHeroGrain(this IGrainFactory factory, string tenant, string key)
			=> factory.GetGrain<IHeroGrain>($"tenant/{tenant}/{key}");

		public static IHeroCollectionGrain GetHeroCollectionGrain(this IGrainFactory factory, string tenant)
			=> factory.GetGrain<IHeroCollectionGrain>($"tenant/{tenant}");

		public static IHeroCategoryGrain GetHeroCategoryGrain(this IGrainFactory factory, string tenant, string key)
			=> factory.GetGrain<IHeroCategoryGrain>($"tenant/{tenant}/{key}");

		public static IHeroCategoryCollectionGrain GetHeroCategoryCollectionGrain(this IGrainFactory factory, string tenant)
			=> factory.GetGrain<IHeroCategoryCollectionGrain>($"tenant/{tenant}");

		// todo: these should be multi tenant
		public static IHeroAbilitiesGrain GetHeroAbilitiesGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(key);

		// todo: these should be multi tenant
		public static IHeroStatsGrain GetHeroStatsGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}