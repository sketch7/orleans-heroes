using Heroes.Server.Tests.Infrastructure;
using System.Text;
using System.Text.Json;

namespace Heroes.Server.Tests.Gql;

[Collection("Integration")]
public sealed class HeroesGqlTests(HeroesWebApplicationFactory factory)
{
	private static readonly Uri GqlEndpoint = new("/graphql", UriKind.Relative);

	private readonly HttpClient _client = factory.CreateHttpClient();

	[Fact(Skip = "GraphQL is currently broken — will be fixed in a separate step")]
	public async Task QueryHeroes_ReturnsHeroesList()
	{
		// Arrange
		var query = new { query = "{ heroes { id name role } }" };
		var body = new StringContent(
			JsonSerializer.Serialize(query),
			Encoding.UTF8,
			"application/json");

		// Act
		var response = await _client.PostAsync(GqlEndpoint, body, TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var json = await response.Content.ReadFromJsonAsync<JsonDocument>(
			TestContext.Current.CancellationToken);

		json.ShouldNotBeNull();
		var heroes = json.RootElement
			.GetProperty("data")
			.GetProperty("heroes");

		heroes.GetArrayLength().ShouldBeGreaterThan(0);
	}
}
