using Heroes.Contracts;
using Heroes.Core.Hosting;
using Heroes.Server.Infrastructure;
using Orleans.Runtime;
using Serilog;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Heroes.Server;

/// <summary>
/// Extension methods for bootstrapping the Heroes server on <see cref="WebApplicationBuilder"/> —
/// mirrors the Arcane <c>UseArcaneServer</c> pattern without any dependency on Arcane libraries.
/// </summary>
public static class HeroesServerHostExtensions
{
	private static readonly ConditionalWeakTable<WebApplicationBuilder, HeroesServerBuilder> HeroesServerBuilders = new();
	private const string HostPropKey = "HeroesHost";

	extension(WebApplicationBuilder builder)
	{
		/// <summary>
		/// Configures a <see cref="WebApplicationBuilder"/> as the Heroes Orleans server.
		/// Sets up the config chain, Serilog, <see cref="IAppInfo"/>, core DI services,
		/// and the Orleans silo via <c>builder.Host.UseOrleans</c>.
		/// </summary>
		/// <param name="args">Command-line arguments passed to the application entry point.</param>
		/// <param name="configure">Optional action to further configure the <see cref="HeroesServerBuilder"/>.</param>
		public WebApplicationBuilder UseHeroesServer(
			string[] args,
			Action<HeroesServerBuilder>? configure = null
		)
		{
			var heroesBuilder = HeroesServerBuilders.GetValue(builder, static b => new(b));

			// Guard against double-initialization — subsequent calls still accumulate delegates.
			if (builder.Host.Properties.ContainsKey(HostPropKey))
			{
				configure?.Invoke(heroesBuilder);
				return builder;
			}

			builder.Host.Properties[HostPropKey] = true;

			// ── 1. Config chain ──────────────────────────────────────────────────────────
			var env = builder.Environment;
			var shortEnvName = AppInfo.MapEnvironmentName(env.EnvironmentName);

			builder.Configuration.Sources.Clear();
			builder.Configuration
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
				.AddJsonFile("app-info.json")
				.AddEnvironmentVariables()
				.AddCommandLine(args);

			var appInfo = new AppInfo(builder.Configuration);

			if (appInfo.IsDockerized)
			{
				builder.Configuration.Sources.Clear();
				builder.Configuration
					.SetBasePath(env.ContentRootPath)
					.AddJsonFile("appsettings.json")
					.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
					.AddJsonFile("appsettings.dev-docker.json", optional: true)
					.AddJsonFile("app-info.json")
					.AddEnvironmentVariables()
					.AddCommandLine(args);

				appInfo = new AppInfo(builder.Configuration);
			}

			heroesBuilder.AppInfo = appInfo;

			// ── 2. Invoke configure callback (AppInfo is live here) ──────────────────────
			configure?.Invoke(heroesBuilder);

			// ── 3. Serilog ───────────────────────────────────────────────────────────────
			builder.Host.UseSerilog((ctx, loggerConfig) =>
			{
				loggerConfig.Enrich.FromLogContext()
					.ReadFrom.Configuration(ctx.Configuration)
					.Enrich.WithMachineName()
					.Enrich.WithDemystifiedStackTraces()
					.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");

				loggerConfig.WithAppInfo(appInfo);
			});

			// ── 4. Core services ─────────────────────────────────────────────────────────
			ConsoleTitleBuilder.Set($"{appInfo.Name} - {appInfo.Environment}");

			builder.Services.AddSingleton<IAppInfo>(appInfo);
			builder.Services.AddSingleton<IAppTenantRegistry, AppTenantRegistry>();
			builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);

			heroesBuilder.ConfigureServicesDelegate?.Invoke(builder.Services);

			// ── 5. Orleans ───────────────────────────────────────────────────────────────
			builder.Host.UseOrleans((ctx, siloBuilder) =>
			{
				siloBuilder
					.ConfigureServices(services => heroesBuilder.ConfigureServicesDelegate?.Invoke(services))
					.AddMemoryStreams(OrleansConstants.STREAM_PROVIDER)
					.AddMemoryGrainStorage("PubSubStore")
					.UseAppConfiguration(new AppSiloBuilderContext
					{
						AppInfo = appInfo,
						HostBuilderContext = ctx,
						SiloOptions = new AppSiloOptions
						{
							SiloPort = GetAvailablePort(11111, 12000),
							GatewayPort = 30001,
						}
					})
					.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
					.AddStartupTask<WarmupStartupTask>();

				heroesBuilder.ConfigureOrleansDelegate?.Invoke(siloBuilder);
			});

			return builder;
		}

		/// <summary>
		/// Retrieves the <see cref="HeroesServerBuilder"/> associated with <paramref name="builder"/>
		/// after a previous call to <see cref="UseHeroesServer"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <see cref="UseHeroesServer"/> has not been called yet.
		/// </exception>
		public HeroesServerBuilder GetHeroesServerBuilder()
			=> HeroesServerBuilders.TryGetValue(builder, out var heroes)
				? heroes
				: throw new InvalidOperationException(
					$"Call {nameof(UseHeroesServer)} before accessing {nameof(HeroesServerBuilder)}."
				);
	}

	private static int GetAvailablePort(int start, int end)
	{
		for (var port = start; port < end; ++port)
		{
			var listener = TcpListener.Create(port);
			listener.ExclusiveAddressUse = true;
			try
			{
				listener.Start();
				return port;
			}
			catch (SocketException)
			{
			}
			finally
			{
				listener.Stop();
			}
		}

		throw new InvalidOperationException($"No available port found in range [{start}, {end}).");
	}
}
