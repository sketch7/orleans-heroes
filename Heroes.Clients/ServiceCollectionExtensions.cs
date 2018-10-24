using Heroes.Clients.Heroes;
using Heroes.Clients.Statistics;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Stats;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Clients
{
	public static class ServiceCollectionExtensions
	{
		public static void AddAppClients(this IServiceCollection services)
		{
			services.AddScoped<IHeroClient, HeroClient>();
			services.AddScoped<IHeroStatsClient, HeroStatsClient>();
		}
	}
}