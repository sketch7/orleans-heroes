using Heroes.Contracts;
using Heroes.Core.Tenancy;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Heroes.GrainClients;
using Heroes.Server.Gql;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using System.Net.Sockets;

namespace Heroes.Server;

public class Program
{
	public static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(new WebApplicationOptions
		{
			Args = args,
			ContentRootPath = Directory.GetCurrentDirectory()
		});

	// Configuration
	var shortEnvName = AppInfo.MapEnvironmentName(builder.Environment.EnvironmentName);
	builder.Configuration
		.AddJsonFile("appsettings.json", optional: true)
		.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
		.AddJsonFile("app-info.json", optional: true)
		.AddEnvironmentVariables()
		.AddCommandLine(args);

	// Build interim AppInfo to determine dockerization and finalize config
	var appInfo = new AppInfo(builder.Configuration);
	if (appInfo.IsDockerized)
	{
		builder.Configuration.Sources.Clear();
		builder.Configuration
			.AddJsonFile("appsettings.json", optional: true)
			.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
			.AddJsonFile("appsettings.dev-docker.json", optional: true)
			.AddJsonFile("app-info.json", optional: true)
			.AddEnvironmentVariables()
			.AddCommandLine(args);
		appInfo = new AppInfo(builder.Configuration);
	}

		Console.Title = $"{appInfo.Name} - {appInfo.Environment}";

		// Logging
		builder.Host.UseSerilog((ctx, loggerConfig) =>
		{
			loggerConfig.Enrich.FromLogContext()
				.ReadFrom.Configuration(ctx.Configuration)
				.Enrich.WithMachineName()
				.Enrich.WithDemystifiedStackTraces()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");

			loggerConfig.WithAppInfo(appInfo);
		});

		// Orleans
		builder.Host.UseOrleans((ctx, siloBuilder) =>
		{
			siloBuilder
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
						// StorageProviderType = StorageProviderType.Redis
					}
				})
				.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
				//.AddOutgoingGrainCallFilter<LoggingOutgoingCallFilter>()
				.AddStartupTask<WarmupStartupTask>()
				.UseSignalR(cfg =>
				{
					cfg.Configure((siloBuilderInner, signalrBuilderConfig) =>
					{
						siloBuilderInner.UseStorage(signalrBuilderConfig.StorageProvider, appInfo, storeName: "SignalR");
					});
				})
				.ConfigureServices(services =>
				{
					services.AddAppGrains(); // Register grain dependencies in Orleans silo
				})
				;
		});

		// Web host and services
		builder.WebHost.UseUrls("http://localhost:6600");

		var services = builder.Services;
		services.AddSingleton(appInfo);
		services.AddSingleton<IAppTenantRegistry, AppTenantRegistry>();
		services.AddAppGrains();

	// Configure tenant-specific services (previously in Grace DI)
	ConfigureServices(services);

		services.Configure<ConsoleLifetimeOptions>(options =>
		{
			options.SuppressStatusMessages = true;
		});

		services.AddSingleton<IHeroService, HeroService>();
		services.AddCustomAuthentication();
		services.AddSignalR(options => options.KeepAliveInterval = TimeSpan.FromSeconds(10))
			.AddJsonProtocol(opts =>
			{
				opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			})
			.AddOrleans();

	services.AddCors(o => o.AddPolicy("TempCorsPolicy", corsBuilder =>
	{
		corsBuilder
			.SetIsOriginAllowed(_ => true)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	}));

		// services.Configure<KestrelServerOptions>(options =>
		// {
		// 	options.AllowSynchronousIO = true;
		// });

		services.AddAppClients();
		services.AddAppGraphQL();
		services.AddControllers().AddNewtonsoftJson();

		var app = builder.Build();

		// Middleware pipeline
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseCors("TempCorsPolicy");
		app.UseGraphQL("/graphql");
		app.UseGraphQLPlayground("/", new()
		{
			GraphQLEndPoint = "/graphql",
			SubscriptionsEndPoint = "/graphql",
		});

		app.UseRouting();
		app.UseAuthorization();

		// Map endpoints directly (UseEndpoints is obsolete in minimal hosting)
		app.MapHub<HeroHub>("/real-time/hero");
		app.MapHub<UserNotificationHub>("/userNotifications");
		app.MapControllers();

		return app.RunAsync();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		// Register TenantGrainActivator for Orleans
		services.AddSingleton<IGrainActivator, TenantGrainActivator>();

		// Register a tenant context provider service
		// Note: This is a simplified implementation. In production, you'd want to:
		// 1. Create an ITenantContextProvider interface
		// 2. Implement tenant resolution from HttpContext, claims, or headers
		// 3. Use middleware to set the current tenant per request
		services.AddScoped<ITenant>(sp =>
		{
			// For now, return a default tenant. This needs proper tenant resolution logic.
			// You'll need to implement proper tenant context management based on your requirements.
			var tenantRegistry = sp.GetRequiredService<IAppTenantRegistry>();
			// Return a default tenant or implement tenant resolution logic here
			return tenantRegistry.GetAll().FirstOrDefault()
				?? throw new InvalidOperationException("No tenants configured");
		});

		// Register tenant-specific services
		// Note: Microsoft DI doesn't support tenant-scoped registrations like Grace DI did.
		// You'll need to implement tenant resolution through factory patterns or strategy patterns.
		services.AddAppLoLGrains();
		services.AddAppHotsGrains();
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

		throw new InvalidOperationException();
	}
}