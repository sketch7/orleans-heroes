using System;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using SignalR.Orleans;

namespace Heroes.Grains
{
	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class HeroGrain : Grain<HeroState>, IHeroGrain
	{
		private const string Source = nameof(HeroGrain);
		private readonly Random _random = new Random();
		private readonly ILogger<HeroGrain> _logger;

		public HeroGrain(
			ILogger<HeroGrain> logger
		)
		{
			_logger = logger;
		}

		public Task Set(Hero hero)
		{
			State.Hero = hero;
			return WriteStateAsync();
		}

		public Task<Hero> Get()
		{
			return Task.FromResult(State.Hero);
		}

		public override async Task OnActivateAsync()
		{
			Console.WriteLine($"{Source} :: OnActivateAsync PK {this.GetPrimaryKeyString()}");
			_logger.LogInformation("{Source} :: OnActivateAsync PK {PK}", Source, this.GetPrimaryKeyString());
			var item = _service.GetById(this.GetPrimaryKeyString());
			await Set(item);

			var streamProvider = GetStreamProvider(Constants.STREAM_PROVIDER);
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{this.GetPrimaryKeyString()}");

			RegisterTimer(async x =>
			{
				State.Hero.Health = _random.Next(100);

				await Task.WhenAll(
					Set(State.Hero),
					stream.OnNextAsync(State.Hero)
				);

			}, State, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
		}

		public override Task OnDeactivateAsync()
		{
			Console.WriteLine($"{Source} :: OnDeactivateAsync PK {this.GetPrimaryKeyString()}");
			return Task.CompletedTask;
		}

	}
}