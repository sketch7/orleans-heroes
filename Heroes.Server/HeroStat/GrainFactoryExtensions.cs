#pragma warning disable IDE0130 // Namespace intentionally matches extended type not folder
namespace Heroes.Server;

public static class HeroStatGrainFactoryExtensions
{
	extension(IGrainFactory factory)
	{
		// todo: make multi-tenant
		public IHeroStatsGrain GetHeroStatsGrain(string key)
			=> factory.GetGrain<IHeroStatsGrain>(key);
	}
}
