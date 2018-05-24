using System;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using SignalR.Orleans;

namespace Heroes.Grains
{
	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroGrain : AppGrain<HeroState>, IHeroGrain
	{
		private readonly IHeroDataClient _heroDataClient;

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
			if (State.Hero == null)
			{
				var hero = await _heroDataClient.GetByKey(PrimaryKey);
				await Set(hero);
			}

			var streamProvider = GetStreamProvider(Constants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{PrimaryKey}");

			RegisterTimer(async x =>
			{
				State.Hero.Health = RandomUtils.GenerateNumber(1, 100);

				await Task.WhenAll(
					Set(State.Hero),
					stream.OnNextAsync(State.Hero)
				);

			}, State, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
		}
		
		public Task Set(Hero hero)
		{
			State.Hero = hero;
			return WriteStateAsync();
		}

		public Task<Hero> Get() => Task.FromResult(State.Hero);

	}
}