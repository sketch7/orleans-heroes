using Orleans.Streams;
using SignalR.Orleans.Core;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.Hero;

[GenerateSerializer]
public sealed class HeroState
{
	[Id(0)]
	public HeroModel? Entity { get; set; }
}

public sealed class HeroGrain(
	ILogger<HeroGrain> logger,
	IHeroDataClient heroDataClient,
	[PersistentState("hero", OrleansConstants.GrainMemoryStorage)]
	IPersistentState<HeroState> state
) : AppGrain<HeroState>(logger, state), IHeroGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private TenantGrainKey _keyData;
	private HubContext<IHeroHub>? _hubContext;

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		_keyData = TenantGrainKey.Parse(PrimaryKey);

		Logger.LogInformation("Activating HeroGrain for tenant: {Tenant}, id: {Id}", TenantAccessor.Tenant?.Key, _keyData.GrainKey);

		cancellationToken.ThrowIfCancellationRequested();

		if (State.Entity is null)
		{
			var entity = await heroDataClient.GetByKey(_keyData.GrainKey);

			if (entity is null)
			{
				Logger.LogWarning("Hero not found for id: {Id} in tenant: {Tenant}", _keyData.GrainKey, _keyData.TenantKey);
				return;
			}

			await Set(entity);
		}

		cancellationToken.ThrowIfCancellationRequested();

		// SignalR hub/stream setup is best-effort — if not yet ready at startup, log and skip.
		try
		{
			_hubContext = GrainFactory.GetHub<IHeroHub>();
			var hubGroup = _hubContext.Group($"{_keyData.TenantKey}/hero/{_keyData.GrainKey}");
			var hubAllGroup = _hubContext.Group($"{_keyData.TenantKey}/hero");

			var streamProvider = this.GetStreamProvider(OrleansConstants.StreamProvider);
			var stream = streamProvider.GetStream<HeroModel>(StreamConstants.HeroStream.ToString(), $"hero:{_keyData.GrainKey}");

			if (State.Entity is not null)
			{
				this.RegisterGrainTimer(async x =>
					{
						var updated = x.Entity! with { Health = RandomUtils.GenerateNumber(1, 100) };

						await Task.WhenAll(
							Set(updated),
							stream.OnNextAsync(updated),
							hubGroup.Send("HeroChanged", updated),
							hubAllGroup.Send("HeroChanged", updated)
						);
					}, State, new() { DueTime = TimeSpan.FromSeconds(2), Period = TimeSpan.FromSeconds(3), Interleave = true });
			}
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Hero {Id} (tenant: {Tenant}) activated without real-time support — SignalR/stream setup failed.", _keyData.GrainKey, _keyData.TenantKey);
		}
	}

	public Task<HeroModel?> Get() => Task.FromResult(State.Entity);

	private Task Set(HeroModel hero)
	{
		State.Entity = hero;
		return WriteStateAsync();
	}
}
