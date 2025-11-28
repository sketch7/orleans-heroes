using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Heroes.Contracts;
using Heroes.Core.Tenancy;
using Heroes.Server.Infrastructure;
using Heroes.Server.Realtime;
using Heroes.Server.Sample;
using Heroes.GrainClients;
using Heroes.Server.Gql;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;

namespace Heroes.Server;

public class Program
{
	public static Task Main(string[] args)
	{
		IAppInfo appInfo = null;

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
		appInfo = new AppInfo(((IConfigurationBuilder)builder.Configuration).Build());
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
			appInfo = new AppInfo(((IConfigurationBuilder)builder.Configuration).Build());
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
				.ConfigureServices(services =>
				{
					services.AddAppGrains(); // Register grain dependencies in Orleans silo
				})
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
				;
		});

		// Web host and services
		builder.WebHost.UseUrls("http://localhost:6600");

		var services = builder.Services;
		services.AddSingleton(appInfo);
		services.AddSingleton<IAppTenantRegistry, AppTenantRegistry>();
		services.AddAppGrains();

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
				.SetIsOriginAllowed((host) => true)
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials();
		}));

		services.Configure<KestrelServerOptions>(options =>
		{
			options.AllowSynchronousIO = true;
		});

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

	private static void ConfigureServices(IInjectionScope scope)
	{
		var tenantRegistry = scope.Locate<IAppTenantRegistry>();
		var tenants = tenantRegistry.GetAll().ToList();

		scope.Configure(c =>
		{

			c.Export<TenantGrainActivator>().As<IGrainActivator>().Lifestyle.Singleton();
			//c
			//	//.Export<MockLoLHeroDataClient>()
			//	.Export<MockHotsHeroDataClient>()
			//	.As<IHeroDataClient>()
			//	;
			// todo: use multi tenancy lib
			c.ExportFactory<IExportLocatorScope, ITenant>(exportScope => exportScope.GetTenantContext());

			//c.Export<MockHotsHeroDataClient>().AsKeyed<IHeroDataClient>("hots").Lifestyle.Singleton();
			//c.Export<MockLoLHeroDataClient>().AsKeyed<IHeroDataClient>("lol").Lifestyle.Singleton();
			//c.ExportFactory<IExportLocatorScope, ITenant, IHeroDataClient>((scope, tenant) =>
			//{
			//	var tenant = RequestContext.Get("tenant") ?? tenant?.Key;

			//	if (tenant == null) throw new ArgumentNullException("tenant", "Tenant must be defined");
			//	return scope.Locate<IHeroDataClient>(withKey: tenant);
			//});


			//c.ExportForAllTenants<IHeroDataClient, MockLoLHeroDataClient>(Tenants.All, x => x.Lifestyle.Singleton());

			//c.ForTenant(Tenants.LeageOfLegends).PopulateFrom(x => x.AddHeroesLoLGrains());
			//c.ForTenant(Tenants.HeroesOfTheStorm).PopulateFrom(x => x.AddHeroesHotsGrains());

			c.ForTenants(tenants, tb =>
			{
				tb
					.ForTenant(AppTenantRegistry.LeagueOfLegends.Key, tc => tc.PopulateFrom(x => x.AddAppLoLGrains()))
					.ForTenant(x => x.Key == AppTenantRegistry.HeroesOfTheStorm.Key, tc => tc.PopulateFrom(x => x.AddAppHotsGrains()))
					;
			});

			/*
			 *
			 * // register with filter tenant
			 * c.ForTenants(tenants, tb =>
			 * {
			 *		tb.ForTenant(x => x.Platform == "x").PopulateFrom(x => x.AddHeroesHotsGrains());
			 * });
			 *
			 * // register one per type
			 * c.For<IHeroDataClient>(tb =>
			 * {
			 *		tb.For(x => x.Key == "lol").Use<MockLoLHeroDataClient>();
			 * });
			 *
			 */

		});
	}

	// todo: remove if its possible to register services directly to grace - https://github.com/ipjohnson/Grace/issues/225
	private class GraceServiceProviderFactory : IServiceProviderFactory<IInjectionScope>
	{
		private readonly IInjectionScopeConfiguration _configuration;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="configuration"></param>
		public GraceServiceProviderFactory(IInjectionScopeConfiguration configuration)
		{
			_configuration = configuration ?? new InjectionScopeConfiguration();
		}

		/// <summary>
		/// Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <returns>A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.</returns>
		public IInjectionScope CreateBuilder(IServiceCollection services)
		{
			var container = new DependencyInjectionContainer(_configuration);

			container.Populate(services);

			ConfigureServices(container);

			return container;
		}

		/// <summary>
		/// Creates an <see cref="T:System.IServiceProvider" /> from the container builder.
		/// </summary>
		/// <param name="containerBuilder">The container builder</param>
		/// <returns>An <see cref="T:System.IServiceProvider" /></returns>
		public IServiceProvider CreateServiceProvider(IInjectionScope containerBuilder)
		{
			return containerBuilder.Locate<IServiceProvider>();
		}
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