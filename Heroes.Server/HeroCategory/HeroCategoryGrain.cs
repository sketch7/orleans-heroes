using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.HeroCategory;

[GenerateSerializer]
public sealed class HeroCategoryState
{
	[Id(0)]
	public HeroCategoryModel? Entity { get; set; }
}

public sealed class HeroCategoryGrain : AppGrain<HeroCategoryState>, IHeroCategoryGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;
	private TenantGrainKey _keyData;

	public HeroCategoryGrain(
		ILogger<HeroCategoryGrain> logger,
		IHeroDataClient heroDataClient,
		[PersistentState("heroCategory", OrleansConstants.GrainMemoryStorage)]
		IPersistentState<HeroCategoryState> state
	) : base(logger, state)
	{
		_heroDataClient = heroDataClient;
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);
		_keyData = TenantGrainKey.Parse(PrimaryKey);

		if (State.Entity is null)
		{
			var entity = await _heroDataClient.GetHeroCategoryByKey(_keyData.GrainKey);

			if (entity is null)
				return;

			await Set(entity);
		}
	}

	public Task<HeroCategoryModel?> Get() => Task.FromResult(State.Entity);

	private Task Set(HeroCategoryModel entity)
	{
		State.Entity = entity;
		return WriteStateAsync();
	}
}
