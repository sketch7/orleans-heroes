using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Stats;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.Contracts.Grains.Mocks
{
	public static class MockDataService
	{
		private static readonly List<Hero> MockData = new List<Hero>
		{
			new Hero {Name = "Rengar", Key = "rengar", Role = HeroRoleType.Assassin, Abilities = new HashSet<string> { "savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"}},
			new Hero {Name = "Kha'zix", Key = "kha-zix", Role = HeroRoleType.Assassin, Abilities = new HashSet<string> { "taste-their-fear", "void-spike", "leap", "void-assault"}},
			new Hero {Name = "Singed", Key = "singed", Role = HeroRoleType.Tank, Abilities = new HashSet<string> { "poison-trail", "mega-adhesive", "fling", "insanity-potion"}},
			new Hero {Name = "AAtrox", Key = "aatrox", Role = HeroRoleType.Fighter, Abilities = new HashSet<string> { "darkin-blade", "infernal-chains", "umbral-dash", "world-ender"}},
			new Hero {Name = "Blitzcrank", Key = "aatrox", Role = HeroRoleType.Tank, Abilities = new HashSet<string> { "rocket-grab", "overdrive", "power-fist", "static-field"}},
			new Hero {Name = "Alistar", Key = "alistar", Role = HeroRoleType.Support, Abilities = new HashSet<string> { "pulverize","headbutt","trample","unbreakable-will"}},
			new Hero {Name = "Morgana", Key = "morgana", Role = HeroRoleType.Support, Abilities = new HashSet<string> { "dark-binding","tormented-shadow","black-shield","soul-shackles"}},
		};

		public static List<Hero> GetHeroes()
		{
			return MockData;
		}

		public static Hero GetById(string key)
		{
			return MockData.FirstOrDefault(x => x.Key == key);
		}

		public static List<HeroAbility> GetHeroesAbilities()
		{
			return new List<HeroAbility>
			{
				//rengar
				new HeroAbility{ Id = "savagery", Name = "Savagery", Damage = 120, DamageType = DamageType.AttackDamage},
				new HeroAbility{ Id = "battle-roar", Name = "Battle Roar", Damage = 170, DamageType = DamageType.MagicDamage},
				new HeroAbility{ Id = "bola-strike", Name = "Bola Strike", Damage = 250, DamageType = DamageType.AttackDamage},
				new HeroAbility{ Id = "thrill-of-the-hunt", Name = "Thrill of the Hunt", Damage = 0, DamageType = DamageType.None},
			
				//kha zix
				new HeroAbility{ Id = "taste-their-fear", Name = "Taste Their Fear", Damage = 170, DamageType = DamageType.AttackDamage},
				new HeroAbility{ Id = "void-spike", Name = "Void Spike", Damage = 200, DamageType = DamageType.AttackDamage},
				new HeroAbility{ Id = "leap", Name = "Leap", Damage = 205, DamageType = DamageType.AttackDamage},
				new HeroAbility{ Id = "void-assault", Name = "Void Assault", Damage = 0, DamageType = DamageType.None},

				//singed
				new HeroAbility{ Id = "poison-trail", Name = "Poison Trail", Damage = 70, DamageType = DamageType.MagicDamage},
				new HeroAbility{ Id = "mega-adhesive", Name = "Mega Adhesive", Damage = 0, DamageType = DamageType.None},
				new HeroAbility{ Id = "fling", Name = "Fling", Damage = 110, DamageType = DamageType.MagicDamage},
				new HeroAbility{ Id = "insanity-potion", Name = "Insanity Potion", Damage = 0, DamageType = DamageType.None},
			};
		}

		public static List<HeroStats> GetHeroStats()
		{
			return new List<HeroStats>
			{
				new HeroStats { HeroId = "rengar", BanRate = 20.75M, WinRate = 50.2M, TotalGames = 60},
				new HeroStats { HeroId = "kha-zix", BanRate = 32M, WinRate = 60.2M, TotalGames = 75},
				new HeroStats { HeroId = "singed", BanRate = 10, WinRate = 75.2M, TotalGames = 100},
			};
		}
	}

}