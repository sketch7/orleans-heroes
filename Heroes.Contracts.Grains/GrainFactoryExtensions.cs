using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Stats;
using Orleans;
using Orleans.Runtime;

namespace Heroes.Contracts.Grains
{
	public static class GrainFactoryExtensions
	{
		public static IHeroGrain GetHeroGrain(this IGrainFactory factory, string tenant, string key)
		{
			RequestContext.Set("tenant", tenant);
			return factory.GetGrain<IHeroGrain>($"tenant\\{tenant}\\{key}");
		}

		public static IHeroCollectionGrain GetHeroCollectionGrain(this IGrainFactory factory, string tenant)
		{
			RequestContext.Set("tenant", tenant);
			return factory.GetGrain<IHeroCollectionGrain>($"tenant\\{tenant}");
		}

		public static IHeroAbilitiesGrain GetHeroAbilitiesGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroAbilitiesGrain>(key);

		public static IHeroStatsGrain GetHeroStatsGrain(this IGrainFactory factory, string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}