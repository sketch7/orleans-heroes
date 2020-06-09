﻿using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using SignalR.Orleans;
using SignalR.Orleans.Core;
using System;
using System.Threading.Tasks;

namespace Heroes.Grains.Heroes
{
	public class HeroState
	{
		public Hero Entity { get; set; }
	}

	public struct HeroKeyData
	{
		public string Tenant { get; set; }
		public string Key { get; set; }
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

			// todo: use key data
			var keySplit = PrimaryKey.Split('/');
			_keyData.Tenant = keySplit[1];
			_keyData.Key = keySplit[2];

			if (State.Entity == null)
			{
				var entity = await _heroDataClient.GetByKey(_keyData.Key);

				if (entity == null)
					return;

				await Set(entity);
			}

			var hubGroup = _hubContext.Group($"hero:{_keyData.Key}");
			var hubAllGroup = _hubContext.Group($"hero:all");

			var streamProvider = GetStreamProvider(Constants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{_keyData.Key}");

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