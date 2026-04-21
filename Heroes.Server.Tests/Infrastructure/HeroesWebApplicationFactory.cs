using Heroes.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Heroes.Server.Tests.Infrastructure;

/// <summary>
/// Shared WebApplicationFactory that boots the full Heroes server with an embedded
/// in-memory Orleans silo. Expensive to create — share it via
/// <see cref="IntegrationCollection"/> so Orleans starts once per test run.
/// </summary>
/// <remarks>
/// Implements <see cref="IAsyncLifetime"/> so xUnit v3 calls <see cref="InitializeAsync"/>
/// before any test timeout window opens, giving the Orleans silo time to fully start.
/// </remarks>
public sealed class HeroesWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
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

			// Remove WarmupStartupTask — grains activate lazily on first call in tests,
			// and the warmup task can delay silo startup significantly.
			services.RemoveAll<WarmupStartupTask>();
			var warmupDescriptor = services.FirstOrDefault(d =>
				d.ServiceType == typeof(IStartupTask) &&
				d.ImplementationType == typeof(WarmupStartupTask));
			if (warmupDescriptor is not null)
				services.Remove(warmupDescriptor);
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
		client.Timeout = TimeSpan.FromSeconds(30);
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

	// ---- IAsyncLifetime ----
	// InitializeAsync runs before any test timeout window opens, so the Orleans silo has
	// time to fully start. Without this, the first test's 30-second timeout fires while
	// the silo is still booting.

	async ValueTask IAsyncLifetime.InitializeAsync()
	{
		// Boot the host before any test timeout window opens.
		try
		{
			using var warmup = CreateClient();
			warmup.Timeout = TimeSpan.FromSeconds(90);
			// Lightweight request — ensures the Orleans silo fully starts.
			await warmup.GetAsync("/openapi/v1.json", CancellationToken.None);
		}
		catch
		{
			// Ignore errors — 4xx is fine; we only need the host to boot.
		}
	}

	// ---- Shutdown guard ----
	// The Orleans silo's membership protocol can hang the process for ~11 minutes
	// during teardown.  We schedule a forced exit before handing off to the base
	// disposal, covering both the async path (called by xUnit v3 via IAsyncDisposable)
	// and the sync path (Dispose(bool) called via IDisposable).

	public override ValueTask DisposeAsync()
	{
		ScheduleShutdownTimeout();
		return base.DisposeAsync();
	}

	protected override void Dispose(bool disposing)
	{
		ScheduleShutdownTimeout();
		base.Dispose(disposing);
	}

	/// <summary>
	/// Schedules <see cref="Environment.Exit"/> after 30 seconds on a thread-pool thread.
	/// If the process exits normally before the delay completes the scheduled action is moot.
	/// </summary>
	private static void ScheduleShutdownTimeout() =>
		_ = Task.Delay(TimeSpan.FromSeconds(30))
			.ContinueWith(_ => Environment.Exit(0), TaskScheduler.Default);
}
