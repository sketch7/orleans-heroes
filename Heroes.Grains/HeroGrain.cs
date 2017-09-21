using System;
using Heroes.Contracts.Grains;
using Orleans;
using System.Threading.Tasks;
using Orleans.Providers;

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
			Console.WriteLine($"{Source} :: OnActivateAsync PK {GetPrimaryKey()}");
			return Task.CompletedTask;
		}

		public override Task OnDeactivateAsync()
		{
			Console.WriteLine($"{Source} :: OnDeactivateAsync PK {GetPrimaryKey()}");
			return Task.CompletedTask;
		}

		public string GetPrimaryKey()
		{
			return this.GetPrimaryKeyString();
		}
	}
}