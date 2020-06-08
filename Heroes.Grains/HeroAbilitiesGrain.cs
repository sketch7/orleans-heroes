using Heroes.Contracts.Heroes;
using Heroes.Contracts.Mocks;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Grains
{
	public class HeroAbilitiesState
	{
		public List<HeroAbility> HeroAbilities { get; set; }
	}

	public class HeroAbilitiesGrain : Grain<HeroAbilitiesState>, IHeroAbilitiesGrain
	{
		private const string Source = nameof(HeroAbilitiesGrain);

		public override Task OnActivateAsync()
		{
			Console.WriteLine($"{Source} :: OnActivateAsync PK {this.GetPrimaryKeyString()}");
			var abilities = MockDataService.GetHeroesAbilities().Where(x => x.HeroId == this.GetPrimaryKeyString()).ToList();
			return Set(abilities);
		}

		public override Task OnDeactivateAsync()
		{
			Console.WriteLine($"{Source} :: OnDeactivateAsync PK {this.GetPrimaryKeyString()}");
			return Task.CompletedTask;
		}

		public Task<List<HeroAbility>> Get()
		{
			return Task.FromResult(State.HeroAbilities);
		}

		public Task Set(List<HeroAbility> heroAbilities)
		{
			State.HeroAbilities = heroAbilities;
			return WriteStateAsync();
		}
	}
}