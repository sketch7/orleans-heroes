using Heroes.Clients.Heroes;
using Heroes.Contracts.Grains.Heroes;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Clients
{
	public static class ServiceCollectionExtensions
	{
		public static void AddHeroesClients(this IServiceCollection services)
		{
			services.AddScoped<IHeroClient, HeroClient>();
		}
	}
}