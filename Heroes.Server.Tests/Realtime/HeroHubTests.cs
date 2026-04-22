using Heroes.Contracts.Heroes;
using Heroes.Server.Tests.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;

namespace Heroes.Server.Tests.Realtime;

[Collection("Integration")]
public sealed class HeroHubTests(HeroesWebApplicationFactory factory)
{
	private const string HubPath = "/real-time/hero";

	// ---- connect ----

	[Fact(Timeout = 30_000)]
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

	[Fact(Timeout = 30_000)]
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

	// ---- Send event (FE: send$()) ----

	[Fact(Timeout = 30_000)]
	public async Task OnConnect_Send_BroadcastsJoinedMessage()
	{
		// Arrange
		var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
		await using var connection = factory.CreateHubConnection(HubPath);
		connection.On<string>("Send", msg => tcs.TrySetResult(msg));

		// Act
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Assert — server calls Clients.All.Send("{connectionId} joined") in OnConnectedAsync
		var received = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
		received.ShouldContain("joined");

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}

	// ---- AddToGroup (FE: addToGroup$()) ----

	[Fact(Timeout = 30_000)]
	public async Task AddToGroup_WhenInvoked_DoesNotThrow()
	{
		// Arrange
		await using var connection = factory.CreateHubConnection(HubPath);
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Act & Assert — no exception means the client was added to the group
		await Should.NotThrowAsync(
			() => connection.InvokeAsync("AddToGroup", "lol/hero/rengar", TestContext.Current.CancellationToken));

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}

	// ---- AddToGroups (FE: addToGroups$()) ----

	[Fact(Timeout = 30_000)]
	public async Task AddToGroups_WhenInvoked_DoesNotThrow()
	{
		// Arrange
		await using var connection = factory.CreateHubConnection(HubPath);
		await connection.StartAsync(TestContext.Current.CancellationToken);

		// Act & Assert
		// FE passes string[] which SignalR deserializes into HashSet<string> server-side
		await Should.NotThrowAsync(
			() => connection.InvokeAsync("AddToGroups", new[] { "lol/hero", "lol/hero/rengar" }, TestContext.Current.CancellationToken));

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}

	// ---- HeroChanged event (FE: heroChanged$()) ----

	// The HeroChanged event is dispatched by HeroGrain timers through the
	// SignalR.Orleans backplane (Orleans stream → group grain → client).
	// WebApplicationFactory routes SignalR connections through an in-process
	// HttpMessageHandler which is not fully transparent to the Orleans backplane's
	// connection-tracking; messages dispatched by grains do not reach the
	// in-process test client reliably.  This event is covered by E2E tests
	// against a live server instead.
	[Fact(Skip = "SignalR.Orleans backplane does not deliver grain-dispatched events through WebApplicationFactory's in-process handler — covered by E2E tests")]
	public async Task HeroChanged_AfterJoiningHeroGroup_ReceivesHeroUpdate()
	{
		// Arrange
		var tcs = new TaskCompletionSource<Hero>(TaskCreationOptions.RunContinuationsAsynchronously);
		await using var connection = factory.CreateHubConnection(HubPath);
		connection.On<Hero>("HeroChanged", hero => tcs.TrySetResult(hero));

		await connection.StartAsync(TestContext.Current.CancellationToken);
		await connection.InvokeAsync("AddToGroup", "lol/hero", TestContext.Current.CancellationToken);

		// Act — wait up to 10s for the next grain timer tick
		var hero = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);

		// Assert
		hero.ShouldNotBeNull();
		hero.Id.ShouldNotBeNullOrEmpty();
		hero.Name.ShouldNotBeNullOrEmpty();

		// Cleanup
		await connection.StopAsync(TestContext.Current.CancellationToken);
	}
}
