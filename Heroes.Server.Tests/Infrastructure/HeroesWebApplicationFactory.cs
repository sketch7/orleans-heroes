using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Heroes.Server.Tests.Infrastructure;

/// <summary>
/// Shared WebApplicationFactory that boots the full Heroes server with an embedded
/// in-memory Orleans silo. Expensive to create — share it via
/// <see cref="IntegrationCollection"/> so Orleans starts once per test run.
/// </summary>
public sealed class HeroesWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Use Development so SiloBuilderExtensions picks in-memory storage.
		// Also inject as a config key because AppInfo reads it from IConfiguration
		// rather than IWebHostEnvironment.
		builder.UseEnvironment("Development");
		builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Development");

		builder.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.SetMinimumLevel(LogLevel.Warning);
		});

		builder.ConfigureServices(services =>
		{
			// Cap host shutdown timeout so the Orleans silo doesn't hang the test
			// process for multiple minutes waiting for membership gossip to drain.
			services.Configure<HostOptions>(opts =>
				opts.ShutdownTimeout = TimeSpan.FromSeconds(30));

			// Speed up Orleans membership protocol teardown.
			services.Configure<ClusterMembershipOptions>(opts =>
			{
				opts.IAmAliveTablePublishTimeout = TimeSpan.FromSeconds(2);
				opts.ProbeTimeout = TimeSpan.FromSeconds(1);
				opts.NumMissedProbesLimit = 1;
				opts.TableRefreshTimeout = TimeSpan.FromSeconds(2);
			});
		});
	}

	/// <summary>
	/// Returns an <see cref="HttpClient"/> with the given auth token pre-set in the
	/// <c>token</c> header used by <see cref="Heroes.Server.Infrastructure.CustomAuthenticationHandler"/>.
	/// Pass <c>null</c> (or omit) for an unauthenticated client.
	/// </summary>
	public HttpClient CreateHttpClient(string? tokenKey = null)
	{
		var client = CreateClient();
		if (!string.IsNullOrEmpty(tokenKey))
			client.DefaultRequestHeaders.Add("token", tokenKey);
		return client;
	}

	/// <summary>
	/// Builds a <see cref="HubConnection"/> routed through the in-process test server,
	/// so no real TCP port is required.
	/// </summary>
	public HubConnection CreateHubConnection(string hubPath, string? tokenKey = null)
	{
		var builder = new HubConnectionBuilder()
			.WithUrl(new Uri(Server.BaseAddress, hubPath), options =>
			{
				// Route SignalR through the in-process test server
				options.HttpMessageHandlerFactory = _ => Server.CreateHandler();

				if (!string.IsNullOrEmpty(tokenKey))
					options.Headers.Add("token", tokenKey);
			});

		return builder.Build();
	}
}
