using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Orleans.Streams;
using SignalR.Orleans.Core;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.Hero;

[GenerateSerializer]
public sealed class HeroState
{
	[Id(0)]
	public Hero Entity { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public sealed class HeroGrain : AppGrain<HeroState>, IHeroGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;
	private TenantGrainKey _keyData;
	private HubContext<IHeroHub> _hubContext;

	public HeroGrain(
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

		Logger.LogInformation("Activating HeroGrain for tenant: {Tenant}, id: {Id}", TenantAccessor.Tenant?.Key, _keyData.GrainKey);

		cancellationToken.ThrowIfCancellationRequested();

		if (State.Entity == null)
		{
			var entity = await _heroDataClient.GetByKey(_keyData.GrainKey);

			if (entity == null)
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

			var streamProvider = this.GetStreamProvider(OrleansConstants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream.ToString(), $"hero:{_keyData.GrainKey}");

			if (State.Entity != null)
			{
				this.RegisterGrainTimer(async x =>
					{
						State.Entity.Health = RandomUtils.GenerateNumber(1, 100);

						await Task.WhenAll(
							Set(State.Entity),
							stream.OnNextAsync(State.Entity),
							hubGroup.Send("HeroChanged", State.Entity),
							hubAllGroup.Send("HeroChanged", State.Entity)
						);
					}, State, new GrainTimerCreationOptions { DueTime = TimeSpan.FromSeconds(2), Period = TimeSpan.FromSeconds(3), Interleave = true });
			}
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Hero {Id} (tenant: {Tenant}) activated without real-time support — SignalR/stream setup failed.", _keyData.GrainKey, _keyData.TenantKey);
		}
	}

	public Task<Hero> Get() => Task.FromResult(State.Entity);

	private Task Set(Hero hero)
	{
		State.Entity = hero;
		return WriteStateAsync();
	}
}
