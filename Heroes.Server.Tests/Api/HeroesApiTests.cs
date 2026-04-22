using Heroes.Server.Hero;
using Heroes.Server.Tests.Infrastructure;

namespace Heroes.Server.Tests.Api
{
	using Hero = Heroes.Server.Hero.Hero;

	[Collection("Integration")]
	public sealed class HeroesApiTests(HeroesWebApplicationFactory factory)
	{
		private readonly HttpClient _client = factory.CreateHttpClient();

		// ---- GET /api/{tenant}/heroes ----

		[Theory(Timeout = 30_000)]
		[InlineData("lol")]
		[InlineData("hots")]
		public async Task GetAll_WithValidTenant_ReturnsOk(string tenant)
		{
			// Act
			var response = await _client.GetAsync($"/api/{tenant}/heroes", TestContext.Current.CancellationToken);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		[Fact(Timeout = 30_000)]
		public async Task GetAll_LoLTenant_ReturnsNonEmptyList()
		{
			// Act
			var heroes = await _client.GetFromJsonAsync<List<Hero>>(
				"/api/lol/heroes", TestContext.Current.CancellationToken);

			// Assert
			heroes.ShouldNotBeNull();
			heroes.ShouldNotBeEmpty();
		}

		[Fact(Timeout = 30_000)]
		public async Task GetAll_LoLTenant_HeroesHaveExpectedShape()
		{
			// Act
			var heroes = await _client.GetFromJsonAsync<List<Hero>>(
				"/api/lol/heroes", TestContext.Current.CancellationToken);

			// Assert
			heroes.ShouldNotBeNull();
			heroes.ShouldAllBe(h => !string.IsNullOrEmpty(h.Id));
			heroes.ShouldAllBe(h => !string.IsNullOrEmpty(h.Name));
		}

		[Fact(Timeout = 30_000)]
		public async Task GetAll_HotsTenant_ReturnsNonEmptyList()
		{
			// Act
			var heroes = await _client.GetFromJsonAsync<List<Hero>>(
				"/api/hots/heroes", TestContext.Current.CancellationToken);

			// Assert
			heroes.ShouldNotBeNull();
			heroes.ShouldNotBeEmpty();
		}

		[Fact(Timeout = 30_000)]
		public async Task GetAll_UnknownTenant_ReturnsBadRequest()
		{
			// Act
			var response = await _client.GetAsync("/api/unknown-tenant/heroes", TestContext.Current.CancellationToken);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
		}

		// ---- GET /api/{tenant}/heroes/{id} ----

		[Fact(Timeout = 30_000)]
		public async Task Get_WithValidHeroId_ReturnsOk()
		{
			// Act
			var response = await _client.GetAsync("/api/lol/heroes/rengar", TestContext.Current.CancellationToken);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		[Fact(Timeout = 30_000)]
		public async Task Get_WithValidHeroId_ReturnsCorrectHero()
		{
			// Act
			var hero = await _client.GetFromJsonAsync<Hero>(
				"/api/lol/heroes/rengar", TestContext.Current.CancellationToken);

			// Assert
			hero.ShouldNotBeNull();
			hero.Id.ShouldBe("rengar");
			hero.Name.ShouldBe("Rengar");
			hero.Role.ShouldBe(HeroRoleType.Assassin);
		}
	}
}
