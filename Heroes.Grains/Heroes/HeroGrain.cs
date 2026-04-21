using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Orleans.Streams;
using SignalR.Orleans.Core;
using Sketch7.Multitenancy;
using Sketch7.Multitenancy.Orleans;
using System.Diagnostics;

namespace Heroes.Grains.Heroes;

[GenerateSerializer]
public class HeroState
{
	[Id(0)]
	public Hero Entity { get; set; }
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct HeroKeyData
{
	private string DebuggerDisplay => $"Tenant: '{Tenant}', Id: '{Id}'";

	public static string Template = "tenant/{tenant}/{id}";

	public string Tenant { get; set; }
	public string Id { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public class HeroGrain : AppGrain<HeroState>, IHeroGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private readonly IHeroDataClient _heroDataClient;
	private HeroKeyData _keyData;

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

		_keyData = this.ParseKey<HeroKeyData>(HeroKeyData.Template);

		Logger.LogInformation("Activating HeroGrain for tenant: {Tenant}, id: {Id}", TenantAccessor.Tenant?.Key, _keyData.Id);

		// Check cancellation before proceeding
		cancellationToken.ThrowIfCancellationRequested();

		if (State.Entity == null)
		{
			var entity = await _heroDataClient.GetByKey(_keyData.Id);

			if (entity == null)
			{
				Logger.LogWarning("Hero not found for id: {Id} in tenant: {Tenant}", _keyData.Id, _keyData.Tenant);
				return;
			}

			await Set(entity);
		}

		// Check cancellation before setting up SignalR and streams
		cancellationToken.ThrowIfCancellationRequested();

		// SignalR hub/stream setup is best-effort — if the stream provider or hub
		// grains are not yet ready (e.g. early in startup), log and skip real-time
		// features rather than failing grain activation entirely.
		try
		{
			_hubContext = GrainFactory.GetHub<IHeroHub>();
			var hubGroup = _hubContext.Group($"{_keyData.Tenant}/hero/{_keyData.Id}");
			var hubAllGroup = _hubContext.Group($"{_keyData.Tenant}/hero"); // all

			var streamProvider = this.GetStreamProvider(OrleansConstants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream.ToString(), $"hero:{_keyData.Id}");

			// Only register timer if we have a valid entity
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
			Logger.LogWarning(ex, "Hero {Id} (tenant: {Tenant}) activated without real-time support — SignalR/stream setup failed.", _keyData.Id, _keyData.Tenant);
		}
	}

	public Task<Hero> Get() => Task.FromResult(State.Entity);

	private Task Set(Hero hero)
	{
		State.Entity = hero;
		return WriteStateAsync();
	}
}