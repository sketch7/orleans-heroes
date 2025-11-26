using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Streams;
// TODO: Re-enable when SignalR.Orleans supports Orleans 9.x
//using SignalR.Orleans;
//using SignalR.Orleans.Core;
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
public class HeroGrain : AppGrain<HeroState>, IHeroGrain
{
	private readonly IHeroDataClient _heroDataClient;
	private HeroKeyData _keyData;
	// TODO: Re-enable when SignalR.Orleans supports Orleans 9.x
	//private HubContext<IHeroHub> _hubContext;

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
		// TODO: Re-enable when SignalR.Orleans supports Orleans 9.x
		//_hubContext = GrainFactory.GetHub<IHeroHub>();
		_keyData = this.ParseKey<HeroKeyData>(HeroKeyData.Template);

		if (State.Entity == null)
		{
			var entity = await _heroDataClient.GetByKey(_keyData.Id);

			if (entity == null)
				return;

			await Set(entity);
		}

		// TODO: Re-enable when SignalR.Orleans supports Orleans 9.x
		//var hubGroup = _hubContext.Group($"{_keyData.Tenant}/hero/{_keyData.Id}");
		//var hubAllGroup = _hubContext.Group($"{_keyData.Tenant}/hero"); // all

		var streamProvider = this.GetStreamProvider(OrleansConstants.STREAM_PROVIDER);
		var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream.ToString(), $"hero:{_keyData.Id}");

		this.RegisterGrainTimer(async x =>
			{
				State.Entity.Health = RandomUtils.GenerateNumber(1, 100);

				// TODO: Re-enable when SignalR.Orleans supports Orleans 9.x
				await Task.WhenAll(
					Set(State.Entity),
					stream.OnNextAsync(State.Entity)
				//hubGroup.Send("HeroChanged", State.Entity),
				//hubAllGroup.Send("HeroChanged", State.Entity)
				);
			}, State, new GrainTimerCreationOptions { DueTime = TimeSpan.FromSeconds(2), Period = TimeSpan.FromSeconds(3), Interleave = true });
	}

	public Task<Hero> Get() => Task.FromResult(State.Entity);

	private Task Set(Hero hero)
	{
		State.Entity = hero;
		return WriteStateAsync();
	}
}