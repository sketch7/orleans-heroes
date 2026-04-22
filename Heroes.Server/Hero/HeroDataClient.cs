using Microsoft.Extensions.Logging;

namespace Heroes.Server.Hero;

public interface IHeroDataClient
{
	Task<HeroModel> GetByKey(string key);
	Task<List<HeroModel>> GetAll();
	Task<HeroCategoryModel> GetHeroCategoryByKey(string key);
	Task<List<HeroCategoryModel>> GetAllHeroCategory();
}

public sealed class MockLoLHeroDataClient : IHeroDataClient
{
	private static readonly List<HeroModel> Heroes =
	[
		new HeroModel { Name = "Rengar", Id = "rengar", Role = HeroRoleType.Assassin, Abilities = ["savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"], Popularity = 6 },
		new HeroModel { Name = "Kha'zix", Id = "kha-zix", Role = HeroRoleType.Assassin, Abilities = ["taste-their-fear", "void-spike", "leap", "void-assault"], Popularity = 4 },
		new HeroModel { Name = "Singed", Id = "singed", Role = HeroRoleType.Tank, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"], Popularity = 3 },
		new HeroModel { Name = "Aatrox", Id = "aatrox", Role = HeroRoleType.Fighter, Abilities = ["darkin-blade", "infernal-chains", "umbral-dash", "world-ender"], Popularity = 9 },
		new HeroModel { Name = "Blitzcrank", Id = "blitzcrank", Role = HeroRoleType.Tank, Abilities = ["rocket-grab", "overdrive", "power-fist", "static-field"], Popularity = 6 },
		new HeroModel { Name = "Alistar", Id = "alistar", Role = HeroRoleType.Support, Abilities = ["pulverize", "headbutt", "trample", "unbreakable-will"], Popularity = 3 },
		new HeroModel { Name = "Morgana", Id = "morgana", Role = HeroRoleType.Support, Abilities = ["dark-binding", "tormented-shadow", "black-shield", "soul-shackles"], Popularity = 8 },
		new HeroModel { Name = "Garen", Id = "garen", Role = HeroRoleType.Fighter, Abilities = ["decisive-strike", "courage", "judgment", "demacian-justice"], Popularity = 7 },
		new HeroModel { Name = "Ryze", Id = "ryze", Role = HeroRoleType.Mage, Abilities = ["overload", "rune-prison", "spell-flux", "realm-warp"], Popularity = 5 },
	];

	private static readonly List<HeroCategoryModel> HeroCategories =
	[
		new HeroCategoryModel { Id = "featured", Title = "Featured", Heroes = ["kha-zix", "aatrox"] },
		new HeroCategoryModel { Id = "recommended", Title = "Recommended", Heroes = ["garen", "ryze", "aatrox"] },
	];

	private readonly ILogger<MockLoLHeroDataClient> _logger;

	public Guid InstanceId { get; } = Guid.NewGuid();

	public MockLoLHeroDataClient(ILogger<MockLoLHeroDataClient> logger)
	{
		_logger = logger;
	}

	public Task<List<HeroModel>> GetAll()
	{
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
		return Task.FromResult(Heroes.ToList());
	}

	public Task<HeroModel> GetByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
		return Task.FromResult(Heroes.FirstOrDefault(x => x.Id == key));
	}

	public Task<HeroCategoryModel> GetHeroCategoryByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetHeroCategoryByKey), key);
		return Task.FromResult(HeroCategories.FirstOrDefault(x => x.Id == key));
	}

	public Task<List<HeroCategoryModel>> GetAllHeroCategory()
	{
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAllHeroCategory));
		return Task.FromResult(HeroCategories.ToList());
	}
}

public sealed class MockHotsHeroDataClient : IHeroDataClient
{
	private static readonly List<HeroModel> Heroes =
	[
		new HeroModel { Name = "Maiev", Id = "maiev", Role = HeroRoleType.Assassin, Abilities = ["savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"] },
		new HeroModel { Name = "Alexstrasza", Id = "alexstrasza", Role = HeroRoleType.Support, Abilities = ["taste-their-fear", "void-spike", "leap", "void-assault"] },
		new HeroModel { Name = "Malthael", Id = "malthael", Role = HeroRoleType.Assassin, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
		new HeroModel { Name = "Johanna", Id = "johanna", Role = HeroRoleType.Tank, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
		new HeroModel { Name = "Kael'Thas", Id = "keal-thas", Role = HeroRoleType.Assassin, Abilities = ["poison-trail", "mega-adhesive", "fling", "insanity-potion"] },
	];

	private readonly ILogger<MockHotsHeroDataClient> _logger;

	public Guid InstanceId { get; } = Guid.NewGuid();

	public MockHotsHeroDataClient(ILogger<MockHotsHeroDataClient> logger)
	{
		_logger = logger;
	}

	public Task<List<HeroModel>> GetAll()
	{
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
		return Task.FromResult(Heroes.ToList());
	}

	public Task<HeroModel> GetByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
		return Task.FromResult(Heroes.FirstOrDefault(x => x.Id == key));
	}

	public Task<HeroCategoryModel> GetHeroCategoryByKey(string key)
		=> throw new NotImplementedException();

	public Task<List<HeroCategoryModel>> GetAllHeroCategory()
		=> throw new NotImplementedException();
}