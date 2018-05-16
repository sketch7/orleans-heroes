using Heroes.Grains;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{

		public static IServiceCollection AddHeroesGrains(this IServiceCollection services)
		{
			services.AddSingleton<IHeroDataClient, MockLoLHeroDataClient>();
			return services;
		}
	}
}
