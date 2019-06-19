using Heroes.Contracts.Grains.Mocks;
using Heroes.Contracts.Grains.Stats;
using Orleans;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	public class HeroStatsGrain : Grain<HeroStatsState>, IHeroStatsGrain
	{
		private const string Source = nameof(HeroStatsGrain);

		public Task Set(HeroStats hero)
		{
			State.HeroStats = hero;
			return WriteStateAsync();
		}

		public Task<HeroStats> Get()
		{
			return Task.FromResult(State.HeroStats);
		}

		public override Task OnActivateAsync()
		{
			Console.WriteLine($"{Source} :: OnActivateAsync PK {this.GetPrimaryKeyString()}");
			State.HeroStats = MockDataService.GetHeroStats().SingleOrDefault(x => x.HeroId == this.GetPrimaryKeyString());
			return Task.CompletedTask;
		}

		public override Task OnDeactivateAsync()
		{
			Console.WriteLine($"{Source} :: OnDeactivateAsync PK {this.GetPrimaryKeyString()}");
			return Task.CompletedTask;
		}
	}
}