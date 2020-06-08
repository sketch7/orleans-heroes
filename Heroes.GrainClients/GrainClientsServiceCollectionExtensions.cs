using Heroes.Contracts.Heroes;
using Heroes.Contracts.Stats;
using Heroes.GrainClients.Heroes;
using Heroes.GrainClients.Statistics;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.GrainClients
{
	public static class GrainClientsServiceCollectionExtensions
	{
		public static void AddAppClients(this IServiceCollection services)
		{
			services.AddScoped<IHeroGrainClient, HeroGrainClient>();
			services.AddScoped<IHeroStatsGrainClient, HeroStatsGrainClient>();
		}
	}
}