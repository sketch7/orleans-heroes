using Heroes.Server.Tests.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Heroes.Server.Tests.Realtime;

[Collection("Integration")]
public sealed class HeroHubTests(HeroesWebApplicationFactory factory)
{
	private const string HubPath = "/real-time/hero";

	[Fact]
	public async Task Connect_WithAnonymousUser_Succeeds()
	{
		// Arrange
		await using var connection = factory.CreateHubConnection(HubPath);

		// Act
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Assert
		connection.State.ShouldBe(HubConnectionState.Connected);

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task Connect_WithAuthenticatedUser_Succeeds()
	{
		// Arrange
		// "cla-key" resolves to user "clayton" in CustomAuthenticationHandler
		await using var connection = factory.CreateHubConnection(HubPath, tokenKey: "cla-key");

		// Act
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Assert
		connection.State.ShouldBe(HubConnectionState.Connected);

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}
}
