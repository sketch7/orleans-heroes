using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Heroes.Contracts.Grains;
using Heroes.Core;
using Heroes.Core.Tenancy;
using Heroes.Grains;
using Heroes.Server.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Heroes.Server
{
	public class Program
	{
		public static Task Main(string[] args)
		{
			var hostBuilder = new HostBuilder();
			var graceConfig = new InjectionScopeConfiguration
			{
				Behaviors =
				{
					AllowInstanceAndFactoryToReturnNull = true
				}
			};

			IAppInfo appInfo = null;
			hostBuilder
				//.UseGrace(graceConfig)
				.ConfigureHostConfiguration(cfg =>
				{
					cfg.SetBasePath(Directory.GetCurrentDirectory())
						.AddEnvironmentVariables("ASPNETCORE_") // todo: change from ASPNETCORE_?
						.AddCommandLine(args);
				})
				.UseServiceProviderFactory(new GraceServiceProviderFactory(graceConfig))
				.ConfigureServices((ctx, services) =>
				{
					appInfo = new AppInfo(ctx.Configuration); // rebuild it so we ensure we have latest all configs
					Console.Title = $"{appInfo.Name} - {appInfo.Environment}";

					services.AddSingleton(appInfo);
					services.AddSingleton<IAppTenantRegistry, AppTenantRegistry>();
					services.Configure<ApiHostedServiceOptions>(options =>
					{
						options.Port = GetAvailablePort(6600, 6699);
						//options.PathString = "/health";
					});

					services.Configure<ConsoleLifetimeOptions>(options =>
					{
						options.SuppressStatusMessages = true;
					});
				})
				.ConfigureAppConfiguration((ctx, cfg) =>
				{
					var shortEnvName = AppInfo.MapEnvironmentName(ctx.HostingEnvironment.EnvironmentName);
					cfg.AddJsonFile("appsettings.json")
						.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
						.AddJsonFile("app-info.json")
						.AddEnvironmentVariables()
						.AddCommandLine(args);

					appInfo = new AppInfo(cfg.Build());

					if (!appInfo.IsDockerized) return;

					cfg.Sources.Clear();

					cfg.AddJsonFile("appsettings.json")
						.AddJsonFile($"appsettings.{shortEnvName}.json", optional: true)
						.AddJsonFile("appsettings.dev-docker.json", optional: true)
						.AddJsonFile("app-info.json")
						.AddEnvironmentVariables()
						.AddCommandLine(args);
				})
				.UseSerilog((ctx, loggerConfig) =>
				{
					loggerConfig.Enrich.FromLogContext()
						.ReadFrom.Configuration(ctx.Configuration)
						.Enrich.WithMachineName()
						.Enrich.WithDemystifiedStackTraces()
						.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");

					loggerConfig.WithAppInfo(appInfo);
				})
				.UseOrleans((ctx, builder) =>
				{
					builder
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
						.ConfigureApplicationParts(parts => parts
							.AddApplicationPart(typeof(HeroGrain).Assembly).WithReferences()
						)
						.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
						//.AddOutgoingGrainCallFilter<LoggingOutgoingCallFilter>()
						.AddStartupTask<WarmupStartupTask>()
						.UseSignalR(cfg =>
						{
							cfg.Configure((siloBuilder, signalrBuilderConfig) =>
							{
								siloBuilder.UseStorage(signalrBuilderConfig.StorageProvider, appInfo, storeName: "SignalR");
							});
						})
					;

				})
				.ConfigureServices((ctx, services) =>
				{
					services.AddHostedService<ApiHostedService>();
				})
				;

			return hostBuilder.RunConsoleAsync();
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
}