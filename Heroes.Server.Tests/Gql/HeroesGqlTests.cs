using Heroes.Server.Tests.Infrastructure;
using System.Text;
using System.Text.Json;

namespace Heroes.Server.Tests.Gql;

[Collection("Integration")]
public sealed class HeroesGqlTests(HeroesWebApplicationFactory factory)
{
	private static readonly Uri GqlEndpoint = new("/graphql", UriKind.Relative);

	private readonly HttpClient _client = factory.CreateHttpClient();

	[Fact(Timeout = 30_000)]
	public async Task QueryHeroes_ReturnsHeroesList()
	{
		// Arrange
		var query = new { query = "{ heroes { id name role } }" };
		var request = new HttpRequestMessage(HttpMethod.Post, GqlEndpoint)
		{
			Content = new StringContent(
				JsonSerializer.Serialize(query),
				Encoding.UTF8,
				"application/json"),
		};
		request.Headers.Add("X-Tenant", "lol");

		// Act
		var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

		// Assert
		var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK, $"Response body: {responseBody}");

		var json = JsonDocument.Parse(responseBody);

		json.ShouldNotBeNull();
		var heroes = json.RootElement
			.GetProperty("data")
			.GetProperty("heroes");

		heroes.GetArrayLength().ShouldBeGreaterThan(0);
	}
}
