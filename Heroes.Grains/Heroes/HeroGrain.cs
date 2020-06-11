using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using SignalR.Orleans;
using SignalR.Orleans.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Grains.Heroes
{
	public class HeroState
	{
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
		private HubContext<IHeroHub> _hubContext;

		public HeroGrain(
			ILogger<HeroGrain> logger,
			IHeroDataClient heroDataClient
		) : base(logger)
		{
			_heroDataClient = heroDataClient;
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();
			_hubContext = GrainFactory.GetHub<IHeroHub>();
			_keyData = this.ParseKey<HeroKeyData>(HeroKeyData.Template);

			if (State.Entity == null)
			{
				var entity = await _heroDataClient.GetByKey(_keyData.Id);

				if (entity == null)
					return;

				await Set(entity);
			}

			var hubGroup = _hubContext.Group($"hero:{_keyData.Id}");
			var hubAllGroup = _hubContext.Group($"hero:all");

			var streamProvider = GetStreamProvider(Constants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{_keyData.Id}");

			RegisterTimer(async x =>
			{
				State.Entity.Health = RandomUtils.GenerateNumber(1, 100);

				await Task.WhenAll(
					Set(State.Entity),
					stream.OnNextAsync(State.Entity),
					hubGroup.Send("HeroChanged", State.Entity),
					hubAllGroup.Send("HeroChanged", State.Entity)
				);
			}, State, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
		}

		public Task<Hero> Get() => Task.FromResult(State.Entity);

		private Task Set(Hero hero)
		{
			State.Entity = hero;
			return WriteStateAsync();
		}
	}
}