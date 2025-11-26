﻿using Heroes.Contracts.HeroCategories;
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

public class TenantAwareHeroDataClient : IHeroDataClient
{
	private readonly MockLoLHeroDataClient _lolClient;
	private readonly MockHotsHeroDataClient _hotsClient;

	public TenantAwareHeroDataClient(MockLoLHeroDataClient lolClient, MockHotsHeroDataClient hotsClient)
	{
		_lolClient = lolClient;
		_hotsClient = hotsClient;
	}

	private IHeroDataClient GetClientForCurrentTenant()
	{
		// Try to get tenant from Orleans RequestContext
		var tenant = RequestContext.Get("tenant") as string;

		// Route to appropriate client based on tenant
		return tenant switch
		{
			"hots" => _hotsClient,
			"lol" => _lolClient,
			_ => _lolClient // default to LoL
		};
	}

	public Task<Hero> GetByKey(string key) => GetClientForCurrentTenant().GetByKey(key);

	public Task<List<Hero>> GetAll() => GetClientForCurrentTenant().GetAll();

	public Task<HeroCategory> GetHeroCategoryByKey(string key) => GetClientForCurrentTenant().GetHeroCategoryByKey(key);

	public Task<List<HeroCategory>> GetAllHeroCategory() => GetClientForCurrentTenant().GetAllHeroCategory();
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
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
		return Task.FromResult(MockDataService.GetHeroes().ToList());
	}

	public Task<Hero> GetByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
		return Task.FromResult(MockDataService.GetById(key));
	}

	public Task<HeroCategory> GetHeroCategoryByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetHeroCategoryByKey), key);
		return Task.FromResult(MockDataService.GetHeroCategoryById(key));
	}

	public Task<List<HeroCategory>> GetAllHeroCategory()
	{
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAllHeroCategory));
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
		_logger.LogDebug("[{Method}] Fetch from mock service", nameof(GetAll));
		return Task.FromResult(_data);
	}

	public Task<HeroCategory> GetHeroCategoryByKey(string key) => throw new NotImplementedException();
	public Task<List<HeroCategory>> GetAllHeroCategory() => throw new NotImplementedException();

	public Task<Hero> GetByKey(string key)
	{
		_logger.LogDebug("[{Method}] Fetching key: {Key} from mock service", nameof(GetByKey), key);
		return Task.FromResult(_data.FirstOrDefault(x => x.Id == key));
	}
}