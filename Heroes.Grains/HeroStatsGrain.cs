using Heroes.Contracts.Mocks;
using Heroes.Contracts.Stats;
using Orleans;

namespace Heroes.Grains;

[GenerateSerializer]
public class HeroStatsState
{
	[Id(0)]
	public HeroStats HeroStats { get; set; }
}

// todo: rewrite if its used
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

	public override Task OnActivateAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine($"{Source} :: OnActivateAsync PK {this.GetPrimaryKeyString()}");
		State.HeroStats = MockDataService.GetHeroStats().SingleOrDefault(x => x.HeroId == this.GetPrimaryKeyString());
		return Task.CompletedTask;
	}

	public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
	{
		Console.WriteLine($"{Source} :: OnDeactivateAsync PK {this.GetPrimaryKeyString()} (reason: {reason})");
		return Task.CompletedTask;
	}
}