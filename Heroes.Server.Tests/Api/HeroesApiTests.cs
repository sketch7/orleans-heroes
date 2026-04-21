using Heroes.Contracts.Heroes;
using Heroes.Server.Tests.Infrastructure;

namespace Heroes.Server.Tests.Api;

[Collection("Integration")]
public sealed class HeroesApiTests(HeroesWebApplicationFactory factory)
{
	private readonly HttpClient _client = factory.CreateHttpClient();

	[Fact]
	public async Task GetAll_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/heroes", TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetAll_ReturnsNonEmptyList()
	{
		// Act
		var heroes = await _client.GetFromJsonAsync<List<Hero>>(
			"/api/heroes", TestContext.Current.CancellationToken);

		// Assert
		heroes.ShouldNotBeNull();
		heroes.ShouldNotBeEmpty();
	}

	[Fact]
	public async Task GetAll_HeroesHaveExpectedShape()
	{
		// Act
		var heroes = await _client.GetFromJsonAsync<List<Hero>>(
			"/api/heroes", TestContext.Current.CancellationToken);

		// Assert
		heroes.ShouldNotBeNull();
		heroes.ShouldAllBe(h => !string.IsNullOrEmpty(h.Id));
		heroes.ShouldAllBe(h => !string.IsNullOrEmpty(h.Name));
	}

	[Fact]
	public async Task Get_WithValidHeroId_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/heroes/rengar", TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task Get_WithValidHeroId_ReturnsCorrectHero()
	{
		// Act
		var hero = await _client.GetFromJsonAsync<Hero>(
			"/api/heroes/rengar", TestContext.Current.CancellationToken);

		// Assert
		hero.ShouldNotBeNull();
		hero.Id.ShouldBe("rengar");
		hero.Name.ShouldBe("Rengar");
		hero.Role.ShouldBe(HeroRoleType.Assassin);
	}
}
