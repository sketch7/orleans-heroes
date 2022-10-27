using Heroes.Contracts.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace Heroes.Server.Controllers;

[Route("api/[controller]")]
public class HeroesController : Controller
{
	private readonly IHeroGrainClient _client;

	public HeroesController(
		IHeroGrainClient client
	)
	{
		_client = client;
	}
	// GET api/heroes
	[HttpGet]
	public async Task<List<Hero>> Get()
	{
		var result = await _client.GetAll().ConfigureAwait(false);
		return result;
	}

	// GET api/heroes/rengar
	[HttpGet("{id}")]
	public Task<Hero> Get(string id)
	{
		return _client.Get(id);
	}

}