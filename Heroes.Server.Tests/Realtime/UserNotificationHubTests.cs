using Heroes.Server.Tests.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;

namespace Heroes.Server.Tests.Realtime;

[Collection("Integration")]
public sealed class UserNotificationHubTests(HeroesWebApplicationFactory factory)
{
	private const string HubPath = "/userNotifications";

	[Fact]
	public async Task Connect_Succeeds()
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
		// "ste-key" resolves to user "stephen" in CustomAuthenticationHandler
		await using var connection = factory.CreateHubConnection(HubPath, tokenKey: "ste-key");

		// Act
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Assert
		connection.State.ShouldBe(HubConnectionState.Connected);

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}
}
