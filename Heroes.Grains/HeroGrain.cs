using Heroes.Contracts.Grains.Heroes;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	[StorageProvider(ProviderName = "MemoryStore")]
	public class HeroGrain : Grain<HeroState>, IHeroGrain
	{
		private const string Source = nameof(HeroGrain);

		public Task Set(Hero hero)
		{
			State.Hero = hero;
			return WriteStateAsync();
		}

		public Task<Hero> Get()
		{
			return Task.FromResult(State.Hero);
		}

		public override Task OnActivateAsync()
		{
			Console.WriteLine($"{Source} :: OnActivateAsync PK {this.GetPrimaryKeyString()}");
			return Task.CompletedTask;
		}

		public override Task OnDeactivateAsync()
		{
			Console.WriteLine($"{Source} :: OnDeactivateAsync PK {this.GetPrimaryKeyString()}");
			return Task.CompletedTask;
		}

	}
}