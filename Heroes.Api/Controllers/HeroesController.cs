using Heroes.Contracts.Grains.Heroes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Api.Controllers
{
	[Route("api/[controller]")]
	public class HeroesController : Controller
	{
		private readonly IConfiguration _config;
		private readonly IHeroClient _client;

		public HeroesController(
			IConfiguration config,
			IHeroClient client
			)
		{
			_config = config;
			_client = client;
		}
		// GET api/values
		[HttpGet]
		public async Task<List<Hero>> Get()
		{
			return await _client.GetAll();
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public async Task<Hero> Get(string id)
		{
			return await _client.Get(id);
		}

	}
}