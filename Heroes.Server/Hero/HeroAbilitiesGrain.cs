using Orleans.Providers;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.Hero;

// Abilities data for LoL heroes (only heroes with defined abilities are populated)
file static class AbilitiesData
{
	public static readonly List<HeroAbility> All =
	[
		// rengar
		new HeroAbility { Id = "savagery", HeroId = "rengar", Name = "Savagery", Damage = 120, DamageType = DamageType.AttackDamage },
		new HeroAbility { Id = "battle-roar", HeroId = "rengar", Name = "Battle Roar", Damage = 170, DamageType = DamageType.MagicDamage },
		new HeroAbility { Id = "bola-strike", HeroId = "rengar", Name = "Bola Strike", Damage = 250, DamageType = DamageType.AttackDamage },
		new HeroAbility { Id = "thrill-of-the-hunt", HeroId = "rengar", Name = "Thrill of the Hunt", Damage = 0, DamageType = DamageType.None },

		// kha'zix
		new HeroAbility { Id = "taste-their-fear", HeroId = "kha-zix", Name = "Taste Their Fear", Damage = 170, DamageType = DamageType.AttackDamage },
		new HeroAbility { Id = "void-spike", HeroId = "kha-zix", Name = "Void Spike", Damage = 200, DamageType = DamageType.AttackDamage },
		new HeroAbility { Id = "leap", HeroId = "kha-zix", Name = "Leap", Damage = 205, DamageType = DamageType.AttackDamage },
		new HeroAbility { Id = "void-assault", HeroId = "kha-zix", Name = "Void Assault", Damage = 0, DamageType = DamageType.None },

		// singed
		new HeroAbility { Id = "poison-trail", HeroId = "singed", Name = "Poison Trail", Damage = 70, DamageType = DamageType.MagicDamage },
		new HeroAbility { Id = "mega-adhesive", HeroId = "singed", Name = "Mega Adhesive", Damage = 0, DamageType = DamageType.None },
		new HeroAbility { Id = "fling", HeroId = "singed", Name = "Fling", Damage = 110, DamageType = DamageType.MagicDamage },
		new HeroAbility { Id = "insanity-potion", HeroId = "singed", Name = "Insanity Potion", Damage = 0, DamageType = DamageType.None },
	];
}

[GenerateSerializer]
public sealed class HeroAbilitiesState
{
	[Id(0)]
	public List<HeroAbility> HeroAbilities { get; set; }
}

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public sealed class HeroAbilitiesGrain : AppGrain<HeroAbilitiesState>, IHeroAbilitiesGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private TenantGrainKey _keyData;

	public HeroAbilitiesGrain(ILogger<HeroAbilitiesGrain> logger) : base(logger)
	{
	}

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);
		_keyData = TenantGrainKey.Parse(PrimaryKey);

		if (State.HeroAbilities == null)
		{
			var abilities = AbilitiesData.All
				.Where(x => x.HeroId == _keyData.GrainKey)
				.ToList();
			await Set(abilities);
		}
	}

	public Task<List<HeroAbility>> Get()
		=> Task.FromResult(State.HeroAbilities);

	public Task Set(List<HeroAbility> heroAbilities)
	{
		State.HeroAbilities = heroAbilities;
		return WriteStateAsync();
	}
}
