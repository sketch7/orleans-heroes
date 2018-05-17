using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Heroes.Contracts.Grains;
using Heroes.Core;
using Heroes.Grains;
using Heroes.SiloHost.ConsoleApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Serilog;

namespace Heroes.SiloHost.ConsoleApp
{
	public class Program
	{
		private static ILogger _log;
		private static ISiloHost _siloHost;
		private static HostingEnvironment _hostingEnv;
		private static Stopwatch _startupStopwatch;
		private static readonly ManualResetEvent SiloStopped = new ManualResetEvent(false);

		public static void Main(string[] args)
		{
			_log = LoggingConfig.ConfigureSimple()
				.CreateLogger()
				.ForContext<Program>()
				;
			RunMainAsync(args).Ignore();
			SiloStopped.WaitOne();
		}

		private static async Task RunMainAsync(string[] args)
		{
			try
			{
				_startupStopwatch = Stopwatch.StartNew();
				_hostingEnv = new HostingEnvironment();
				var shortEnvName = AppInfo.MapEnvironmentName(_hostingEnv.Environment);
				var configBuilder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddCommandLine(args)
					.AddJsonFile("config.json")
					.AddJsonFile($"config.{shortEnvName}.json")
					.AddJsonFile("app-info.json")
					.AddEnvironmentVariables();

				if (_hostingEnv.IsDockerDev)
					configBuilder.AddJsonFile("config.dev-docker.json", optional: true);

				var config = configBuilder.Build();

				var appInfo = new AppInfo(config);
				Console.Title = $"Silo - {appInfo.Name}";

				var logger = LoggingConfig.Configure(config, appInfo)
					.CreateLogger();

				Log.Logger = logger;
				_log = logger.ForContext<Program>();
				_log.Information("Initializing Silo {appName} ({version}) [{env}]...", appInfo.Name, appInfo.Version, _hostingEnv.Environment);

				_siloHost = BuildSilo(appInfo, logger);
				AssemblyLoadContext.Default.Unloading += context =>
				{
					_log.Information("Assembly unloading...");

					Task.Run(StopSilo);
					SiloStopped.WaitOne();

					_log.Information("Assembly unloaded complete.");
					Log.CloseAndFlush();
				};

				await StartSilo();
			}
			catch (Exception ex)
			{
				_log.Error(ex, "An error has occurred while initializing or starting silo.");
				Log.CloseAndFlush();
			}
		}

		private static ISiloHost BuildSilo(IAppInfo appInfo, ILogger logger)
		{
			var builder = new SiloHostBuilder()
				.UseHeroConfiguration(appInfo, _hostingEnv)
				.ConfigureLogging(logging => logging.AddSerilog(logger, dispose: true))
				.ConfigureApplicationParts(parts => parts
					.AddApplicationPart(typeof(HeroGrain).Assembly).WithReferences()
				)
				//.AddIncomingGrainCallFilter<LoggingIncomingCallFilter>()
				//.AddOutgoingGrainCallFilter<LoggingOutgoingCallFilter>()
				.AddStartupTask<WarmupStartupTask>()
				.UseServiceProviderFactory(ConfigureServices)
				.UseSignalR();

			return builder.Build();
		}

		private static async Task StartSilo()
		{
			_log.Information("Silo initialized in {timeTaken:#.##}s. Starting...", _startupStopwatch.Elapsed.TotalSeconds);

			await _siloHost.StartAsync();
			_startupStopwatch.Stop();

			_log.Information("Successfully started Silo in {timeTaken:#.##}s (total).", _startupStopwatch.Elapsed.TotalSeconds);
		}

		private static async Task StopSilo()
		{
			_log.Information("Stopping Silo...");

			try
			{
				await _siloHost.StopAsync();
			}
			catch (Exception ex)
			{
				_log.Error(ex, "Stopping Silo failed...");
			}

			_log.Information("Silo shutdown.");

			SiloStopped.Set();
		}

		private static IServiceProvider ConfigureServices(IServiceCollection services)
		{
			//services.AddSingleton<IGrainActivator, TenantGrainActivator>();
			var container = new DependencyInjectionContainer(c => c.Behaviors.AllowInstanceAndFactoryToReturnNull = true);

			var providers = container.Populate(services);
			container.Configure(c =>
			{
				c.Export<TenantGrainActivator>().As<IGrainActivator>().Lifestyle.Singleton();
				//c
				//	//.Export<MockLoLHeroDataClient>()
				//	.Export<MockHotsHeroDataClient>()
				//	.As<IHeroDataClient>()
				//	;
				//c.
				c.ExportFactory<IExportLocatorScope, ITenantContext>(
					scope =>
					{
						return (ITenantContext)scope.GetExtraData("TenantContext");
					});

				//c.Export<MockHotsHeroDataClient>().AsKeyed<IHeroDataClient>("hots").Lifestyle.Singleton();
				//c.Export<MockLoLHeroDataClient>().AsKeyed<IHeroDataClient>("lol").Lifestyle.Singleton();

				//c.ExportFactory<IExportLocatorScope, ITenantContext, IHeroDataClient>((scope, tenantContext) =>
				//{
				//	var tenant = RequestContext.Get("tenant") ?? tenantContext?.Key;

				//	if (tenant == null) throw new ArgumentNullException("tenant", "Tenant must be defined");
				//	return scope.Locate<IHeroDataClient>(withKey: tenant);
				//});


				c.ExportForAllTenants<IHeroDataClient, MockLoLHeroDataClient>(Tenants.All, x => x.Lifestyle.Singleton());

				//c.ForTenant(Tenants.LeageOfLegends.Key).PopulateFrom(x => x.AddHeroesLoLGrains());
				//c.ForTenant(Tenants.HeroesOfTheStorm.Key).PopulateFrom(x => x.AddHeroesHotsGrains());

				//c.ExportPerTenantFactory<IHeroDataClient>();


				//// todo: wrap in method?
				//foreach (var tenant in Tenants.All)
				//{

				//	PerTenant(c, (t, cfg) =>
				//	{
				//		if (t.Key == "sketch7")
				//			cfg.Export<SampleHeroService>().As<IHeroService>();
				//	});
				//}
			});


			var ac = providers.GetService<IGrainActivator>();

			return providers;
		}
	}

	public class TenantContainerBuilder
	{
		private IExportRegistrationBlock _exportConfig;
		private string _tenant;

		public TenantContainerBuilder(IExportRegistrationBlock exportConfig, string tenant)
		{
			_exportConfig = exportConfig;
			_tenant = tenant;
		}

		public void PopulateFrom(Action<IServiceCollection> configure, IServiceCollection services = null)
		{
			services = services ?? new ServiceCollection();
			configure(services);

			// handle registrations

			// loop services and register
		}

	}

	public static class GraceExtensions
	{
		public static TenantContainerBuilder ForTenant(this IExportRegistrationBlock config, string tenant)
			=> new TenantContainerBuilder(config, tenant);

		public static IExportRegistrationBlock ExportPerTenantFactory<T>(this IExportRegistrationBlock config)
		{
			config.ExportFactory<IExportLocatorScope, ITenantContext, T>((scope, tenantContext) =>
			{
				var tenant = tenantContext?.Key;

				if (tenant == null) throw new ArgumentNullException("tenant", "Tenant must be defined");
				return scope.Locate<T>(withKey: tenant);
			});
			return config;
		}

		public static IExportRegistrationBlock ExportPerTenantFactoryWeak(this IExportRegistrationBlock config, Type interfaceType)
		{
			MethodInfo openMethod = typeof(GraceExtensions).GetMethod(nameof(ExportPerTenantFactory));
			MethodInfo typedMethod = openMethod.MakeGenericMethod(interfaceType);
			typedMethod.Invoke(null, new object[] { config });
			//config.ExportFactory<IExportLocatorScope, ITenantContext, object>((scope, tenantContext) =>
			//{
			//	var tenant = tenantContext?.Key;

			//	if (tenant == null) throw new ArgumentNullException("tenant", "Tenant must be defined");
			//	return scope.Locate(withKey: tenant, type: interfaceType);
			//});
			return config;
		}

		public static IExportRegistrationBlock ExportForAllTenants<TInterface, TImplementation>(
			this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants,
			Action<IFluentExportStrategyConfiguration> configure = null)
			=> config.ExportForAllTenants(tenants, typeof(TInterface), typeof(TImplementation), configure);
		//public static IExportRegistrationBlock ExportForAllTenants<TInterface, TImplementation>(
		//	this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants,
		//	Action<IFluentExportStrategyConfiguration> configure = null)
		//{
		//	foreach (var tenant in tenants)
		//	{
		//		var exportConfig = config.Export<TImplementation>().AsKeyed<TInterface>(tenant.Key);
		//		//configure?.Invoke(exportConfig);
		//	}

		//	config.ExportPerTenantFactory<TInterface>();

		//	return config;
		//}

		public static IExportRegistrationBlock ExportForAllTenants(this IExportRegistrationBlock config, IEnumerable<ITenantContext> tenants, Type interfaceType, Type implementationType, Action<IFluentExportStrategyConfiguration> configure = null)
		{
			foreach (var tenant in tenants)
			{
				var exportConfig = config.Export(implementationType).AsKeyed(interfaceType, tenant.Key);
				configure?.Invoke(exportConfig);
			}

			config.ExportPerTenantFactoryWeak(interfaceType);

			return config;
		}
	}
}