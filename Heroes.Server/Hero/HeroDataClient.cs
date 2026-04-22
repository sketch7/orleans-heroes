using Microsoft.Extensions.Logging;

namespace Heroes.Server.Hero
{
	using HeroCategory = Heroes.Server.HeroCategory.HeroCategory;

	public interface IHeroDataClient
	{
		Task<Hero> GetByKey(string key);
		Task<List<Hero>> GetAll();
		Task<HeroCategory> GetHeroCategoryByKey(string key);
		Task<List<HeroCategory>> GetAllHeroCategory();
	}

	public sealed class MockLoLHeroDataClient : IHeroDataClient
	{
		private static readonly List<Hero> Heroes =
		[
			new Hero { Name = "Rengar", Id = "rengar", Role = HeroRoleType.Assassin, Abilities = ["savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"], Popularity = 6 },
			new Hero { Name = "Kha'zix", Id = "kha-zix", Role = HeroRoleType.Assassin, Abilities = ["taste-their-fear", "void-spike", "leap", "void-assault"], Popularity = 4 },
			new Hero { Name = "Singed", Id = "singed", Role = HeroRoleType.Tank, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"], Popularity = 3 },
			new Hero { Name = "Aatrox", Id = "aatrox", Role = HeroRoleType.Fighter, Abilities = ["darkin-blade", "infernal-chains", "umbral-dash", "world-ender"], Popularity = 9 },
			new Hero { Name = "Blitzcrank", Id = "blitzcrank", Role = HeroRoleType.Tank, Abilities = ["rocket-grab", "overdrive", "power-fist", "static-field"], Popularity = 6 },
			new Hero { Name = "Alistar", Id = "alistar", Role = HeroRoleType.Support, Abilities = ["pulverize", "headbutt", "trample", "unbreakable-will"], Popularity = 3 },
			new Hero { Name = "Morgana", Id = "morgana", Role = HeroRoleType.Support, Abilities = ["dark-binding", "tormented-shadow", "black-shield", "soul-shackles"], Popularity = 8 },
			new Hero { Name = "Garen", Id = "garen", Role = HeroRoleType.Fighter, Abilities = ["decisive-strike", "courage", "judgment", "demacian-justice"], Popularity = 7 },
			new Hero { Name = "Ryze", Id = "ryze", Role = HeroRoleType.Mage, Abilities = ["overload", "rune-prison", "spell-flux", "realm-warp"], Popularity = 5 },
		];

		private static readonly List<HeroCategory> HeroCategories =
		[
			new HeroCategory { Id = "featured", Title = "Featured", Heroes = ["kha-zix", "aatrox"] },
			new HeroCategory { Id = "recommended", Title = "Recommended", Heroes = ["garen", "ryze", "aatrox"] },
		];

		private readonly ILogger<MockLoLHeroDataClient> _logger;

		public Guid InstanceId { get; } = Guid.NewGuid();

		public MockLoLHeroDataClient(ILogger<MockLoLHeroDataClient> logger)
		{
			_logger = logger;
		}

		public Task<List<Hero>> GetAll()
		{
			_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
			return Task.FromResult(Heroes.ToList());
		}

		public Task<Hero> GetByKey(string key)
		{
			_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
			return Task.FromResult(Heroes.FirstOrDefault(x => x.Id == key));
		}

		public Task<HeroCategory> GetHeroCategoryByKey(string key)
		{
			_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetHeroCategoryByKey), key);
			return Task.FromResult(HeroCategories.FirstOrDefault(x => x.Id == key));
		}

		public Task<List<HeroCategory>> GetAllHeroCategory()
		{
			_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAllHeroCategory));
			return Task.FromResult(HeroCategories.ToList());
		}
	}

	public sealed class MockHotsHeroDataClient : IHeroDataClient
	{
		private static readonly List<Hero> Heroes =
		[
			new Hero { Name = "Maiev", Id = "maiev", Role = HeroRoleType.Assassin, Abilities = ["savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"] },
			new Hero { Name = "Alexstrasza", Id = "alexstrasza", Role = HeroRoleType.Support, Abilities = ["taste-their-fear", "void-spike", "leap", "void-assault"] },
			new Hero { Name = "Malthael", Id = "malthael", Role = HeroRoleType.Assassin, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
			new Hero { Name = "Johanna", Id = "johanna", Role = HeroRoleType.Tank, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
			new Hero { Name = "Kael'Thas", Id = "keal-thas", Role = HeroRoleType.Assassin, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
		];

		private readonly ILogger<MockHotsHeroDataClient> _logger;

		public Guid InstanceId { get; } = Guid.NewGuid();

		public MockHotsHeroDataClient(ILogger<MockHotsHeroDataClient> logger)
		{
			_logger = logger;
		}

		public Task<List<Hero>> GetAll()
		{
			_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
			return Task.FromResult(Heroes.ToList());
		}

		public Task<Hero> GetByKey(string key)
		{
			_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
			return Task.FromResult(Heroes.FirstOrDefault(x => x.Id == key));
		}

		public Task<HeroCategory> GetHeroCategoryByKey(string key)
			=> throw new NotImplementedException();

		public Task<List<HeroCategory>> GetAllHeroCategory()
			=> throw new NotImplementedException();
	}
}
