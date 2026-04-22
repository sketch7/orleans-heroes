using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.Hero;

// Abilities data for LoL heroes (only heroes with defined abilities are populated)
static file class AbilitiesData
{
	public static readonly List<HeroAbility> All =
	[
		// rengar
		new() { Id = "savagery", HeroId = "rengar", Name = "Savagery", Damage = 120, DamageType = DamageType.AttackDamage },
		new() { Id = "battle-roar", HeroId = "rengar", Name = "Battle Roar", Damage = 170, DamageType = DamageType.MagicDamage },
		new() { Id = "bola-strike", HeroId = "rengar", Name = "Bola Strike", Damage = 250, DamageType = DamageType.AttackDamage },
		new() { Id = "thrill-of-the-hunt", HeroId = "rengar", Name = "Thrill of the Hunt", Damage = 0, DamageType = DamageType.None },

		// kha'zix
		new() { Id = "taste-their-fear", HeroId = "kha-zix", Name = "Taste Their Fear", Damage = 170, DamageType = DamageType.AttackDamage },
		new() { Id = "void-spike", HeroId = "kha-zix", Name = "Void Spike", Damage = 200, DamageType = DamageType.AttackDamage },
		new() { Id = "leap", HeroId = "kha-zix", Name = "Leap", Damage = 205, DamageType = DamageType.AttackDamage },
		new() { Id = "void-assault", HeroId = "kha-zix", Name = "Void Assault", Damage = 0, DamageType = DamageType.None },

		// singed
		new() { Id = "poison-trail", HeroId = "singed", Name = "Poison Trail", Damage = 70, DamageType = DamageType.MagicDamage },
		new() { Id = "mega-adhesive", HeroId = "singed", Name = "Mega Adhesive", Damage = 0, DamageType = DamageType.None },
		new() { Id = "fling", HeroId = "singed", Name = "Fling", Damage = 110, DamageType = DamageType.MagicDamage },
		new() { Id = "insanity-potion", HeroId = "singed", Name = "Insanity Potion", Damage = 0, DamageType = DamageType.None },
	];
}

[GenerateSerializer]
public sealed class HeroAbilitiesState
{
	[Id(0)]
	public List<HeroAbility>? HeroAbilities { get; set; }
}

public sealed class HeroAbilitiesGrain(
	ILogger<HeroAbilitiesGrain> logger,
	[PersistentState("heroAbilities", OrleansConstants.GrainMemoryStorage)]
	IPersistentState<HeroAbilitiesState> state
) : AppGrain<HeroAbilitiesState>(logger, state), IHeroAbilitiesGrain, IWithTenantAccessor<AppTenant>
{
	public TenantAccessor<AppTenant> TenantAccessor { get; set; } = new();

	private TenantGrainKey _keyData;

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);
		_keyData = TenantGrainKey.Parse(PrimaryKey);

		if (State.HeroAbilities is null)
		{
			var abilities = AbilitiesData.All
				.Where(x => x.HeroId == _keyData.GrainKey)
				.ToList();
			await Set(abilities);
		}
	}

	public Task<List<HeroAbility>> Get()
		=> Task.FromResult(State.HeroAbilities ?? []);

	public Task Set(List<HeroAbility> heroAbilities)
	{
		State.HeroAbilities = heroAbilities;
		return WriteStateAsync();
	}
}
