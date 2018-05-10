using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Mocks;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Heroes.Grains
{
	public interface IHeroDataClient
	{
		Task<Hero> GetByKey(string key);
		Task<List<Hero>> GetAll();
	}

	public class MockHeroDataClient : IHeroDataClient
	{
		private readonly ILogger<MockHeroDataClient> _logger;

		public MockHeroDataClient(ILogger<MockHeroDataClient> logger)
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
	}
}
