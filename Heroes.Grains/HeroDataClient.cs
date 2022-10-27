using Heroes.Contracts.HeroCategories;
using Heroes.Contracts.Heroes;
using Heroes.Contracts.Mocks;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Heroes.Grains;

public interface IHeroDataClient
{
	Task<Hero> GetByKey(string key);
	Task<List<Hero>> GetAll();
	Task<HeroCategory> GetHeroCategoryByKey(string key);
	Task<List<HeroCategory>> GetAllHeroCategory();
}

public class MockLoLHeroDataClient : IHeroDataClient
{
	private readonly ILogger<MockLoLHeroDataClient> _logger;

	public Guid InstanceId { get; } = Guid.NewGuid();

	public MockLoLHeroDataClient(ILogger<MockLoLHeroDataClient> logger)
	{
		_logger = logger;
	}

	public Task<List<Hero>> GetAll()
	{
		_logger.Debug($"[{nameof(GetAll)}] Fetch from mock service");
		return Task.FromResult(MockDataService.GetHeroes().ToList());
	}

	public Task<Hero> GetByKey(string key)
	{
		_logger.Debug($"[{nameof(GetByKey)}] Fetching key: {key} from mock service", key);
		return Task.FromResult(MockDataService.GetById(key));
	}

	public Task<HeroCategory> GetHeroCategoryByKey(string key)
	{
		_logger.Debug($"[{nameof(GetHeroCategoryByKey)}] Fetching key: {key} from mock service", key);
		return Task.FromResult(MockDataService.GetHeroCategoryById(key));
	}

	public Task<List<HeroCategory>> GetAllHeroCategory()
	{
		_logger.Debug($"[{nameof(GetAllHeroCategory)}] Fetch from mock service");
		return Task.FromResult(MockDataService.GetAllHeroCategory());
	}
}

public class MockHotsHeroDataClient : IHeroDataClient
{
	private readonly ILogger<MockHotsHeroDataClient> _logger;
	private readonly List<Hero> _data = new List<Hero>
	{
		new Hero {Name = "Maiev", Id = "maiev", Role = HeroRoleType.Assassin, Abilities = new HashSet<string> { "savagery", "battle-roar", "bola-strike", "thrill-of-the-hunt"}},
		new Hero {Name = "Alexstrasza", Id = "alexstrasza", Role = HeroRoleType.Support, Abilities = new HashSet<string> { "taste-their-fear", "void-spike", "leap", "void-assault"}},
		new Hero {Name = "Malthael", Id = "malthael", Role = HeroRoleType.Assassin, Abilities = new HashSet<string> { "poison-trail", "mega-adhesive", "fling", "insanity-potion"}},
		new Hero {Name = "Johanna", Id = "johanna", Role = HeroRoleType.Tank, Abilities = new HashSet<string> { "poison-trail", "mega-adhesive", "fling", "insanity-potion"}},
		new Hero {Name = "Kael'Thas", Id = "keal-thas", Role = HeroRoleType.Assassin, Abilities = new HashSet<string> { "poison-trail", "mega-adhesive", "fling", "insanity-potion"}},
	};

	public Guid InstanceId { get; } = Guid.NewGuid();

	public MockHotsHeroDataClient(ILogger<MockHotsHeroDataClient> logger)
	{
		_logger = logger;
	}

	public Task<List<Hero>> GetAll()
	{
		_logger.Debug($"[{nameof(GetAll)}] Fetch from mock service");
		return Task.FromResult(_data);
	}

	public Task<HeroCategory> GetHeroCategoryByKey(string key) => throw new NotImplementedException();
	public Task<List<HeroCategory>> GetAllHeroCategory() => throw new NotImplementedException();

	public Task<Hero> GetByKey(string key)
	{
		_logger.Debug($"[{nameof(GetByKey)}] Fetching key: {key} from mock service", key);
		return Task.FromResult(_data.FirstOrDefault(x => x.Id == key));
	}
}