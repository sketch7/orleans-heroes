﻿using Heroes.Contracts;
using Heroes.Contracts.HeroCategories;
using Heroes.Core.Orleans;
using Heroes.Grains.Heroes;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Grains.HeroCategories;

[GenerateSerializer]
public class HeroCategoryState
{
	[Id(0)]
	public HeroCategory Entity { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public class HeroCategoryGrain : AppGrain<HeroCategoryState>, IHeroCategoryGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;
	private TenantGrainKey _keyData;

	public HeroCategoryGrain(
		ILogger<HeroGrain> logger,
		IHeroDataClient heroDataClient
	) : base(logger)
	{
		_heroDataClient = heroDataClient;
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);
		_keyData = TenantGrainKey.Parse(PrimaryKey);

		if (State.Entity == null)
		{
			var entity = await _heroDataClient.GetHeroCategoryByKey(_keyData.GrainKey);

			if (entity == null)
				return;

			await Set(entity);
		}
	}

	public Task<HeroCategory> Get() => Task.FromResult(State.Entity);

	private Task Set(HeroCategory entity)
	{
		State.Entity = entity;
		return WriteStateAsync();
	}
}