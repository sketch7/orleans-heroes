using Heroes.Grains;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
	public static class AppGrainsServiceCollectionExtensions
	{

		public static IServiceCollection AddAppGrains(this IServiceCollection services)
		{
			services.AddSingleton<IHeroDataClient, MockLoLHeroDataClient>();
			return services;
		}

		public static IServiceCollection AddAppHotsGrains(this IServiceCollection services)
		{
			services.AddSingleton<IHeroDataClient, MockHotsHeroDataClient>();
			return services;
		}

		public static IServiceCollection AddAppLoLGrains(this IServiceCollection services)
		{
			services.AddSingleton<IHeroDataClient, MockLoLHeroDataClient>();
			return services;
		}
	}
}
