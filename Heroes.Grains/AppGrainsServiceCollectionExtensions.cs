﻿using Heroes.Grains;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AppGrainsServiceCollectionExtensions
{

	public static IServiceCollection AddAppGrains(this IServiceCollection services)
	{
		// Register both tenant-specific implementations
		services.AddSingleton<MockLoLHeroDataClient>();
		services.AddSingleton<MockHotsHeroDataClient>();

		// Register a factory that resolves based on tenant context
		services.AddSingleton<IHeroDataClient>(sp =>
		{
			var lolClient = sp.GetRequiredService<MockLoLHeroDataClient>();
			var hotsClient = sp.GetRequiredService<MockHotsHeroDataClient>();
			return new TenantAwareHeroDataClient(lolClient, hotsClient);
		});

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