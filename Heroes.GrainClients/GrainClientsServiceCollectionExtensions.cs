using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Contracts.Stats;
using Heroes.GrainClients.HeroCategories;
using Heroes.GrainClients.Heroes;
using Heroes.GrainClients.Statistics;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.GrainClients
{
	public static class GrainClientsServiceCollectionExtensions
	{
		public static void AddAppClients(this IServiceCollection services)
		{
			services.AddSingleton<IHeroCategoryGrainClient, HeroCategoryGrainClient>();
			services.AddSingleton<IHeroGrainClient, HeroGrainClient>();
			services.AddSingleton<IHeroStatsGrainClient, HeroStatsGrainClient>();
		}
	}
}