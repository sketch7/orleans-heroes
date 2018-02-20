using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Heroes.Core;
using Heroes.Grains;
using Heroes.SiloHost.ConsoleApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using Serilog;

namespace Heroes.SiloHost.ConsoleApp
{
	public class Program
	{
		private static ILogger _log;
		private static ISiloHost _siloHost;
		private static HostingEnvironment _hostingEnvironment;
		private static readonly ManualResetEvent SiloStopped = new ManualResetEvent(false);
		private static Stopwatch _startupStopwatch;

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
				_hostingEnvironment = new HostingEnvironment();
				var shortEnvName = AppInfo.MapEnvironmentName(_hostingEnvironment.Environment);
				var configBuilder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddCommandLine(args)
					.AddJsonFile("config.json")
					.AddJsonFile($"config.{shortEnvName}.json")
					.AddJsonFile("app-info.json")
					.AddEnvironmentVariables();

				if (_hostingEnvironment.IsDockerDev)
					configBuilder.AddJsonFile("config.dev-docker.json", optional: true);

				var config = configBuilder.Build();

				var appInfo = new AppInfo(config);
				Console.Title = $"Silo - {appInfo.Name}";

				var logger = LoggingConfig.Configure(config, appInfo)
					.CreateLogger();

				Log.Logger = logger;
				_log = logger.ForContext<Program>();
				_log.Information("Initializing Silo {appName} ({version}) [{env}]...", appInfo.Name, appInfo.Version, _hostingEnvironment.Environment);

				_siloHost = BuildSilo(logger);
				AssemblyLoadContext.Default.Unloading += context =>
				{
					_log.Information("Assembly unloading...");

					Task.Run(() => StopSilo(appInfo));
					SiloStopped.WaitOne();

					_log.Information("Assembly unloaded complete.");
				};

				await StartSilo();
			}
			catch (Exception ex)
			{
				_log.Error(ex, "An error has occurred while initializing or starting silo.");
			}
		}

		private static ISiloHost BuildSilo(ILogger logger)
		{
			var config = ClusterConfiguration.LocalhostPrimarySilo();
			config.AddMemoryStorageProvider();

			var builder = new SiloHostBuilder()
				.UseConfiguration(config)
				.ConfigureLogging(logging => logging.AddSerilog(logger, dispose: true))
				.ConfigureApplicationParts(parts => parts
					.AddApplicationPart(typeof(HeroGrain).Assembly).WithReferences()
				)
				//.UseServiceProviderFactory(services =>
				//{
				//	return services.AddSingleton(...);
				//})
				;

			return builder.Build();
		}

		private static async Task StartSilo()
		{
			_log.Information("Silo initialized in {timeTaken:#.##}s. Starting...", _startupStopwatch.Elapsed.TotalSeconds);

			await _siloHost.StartAsync();
			_startupStopwatch.Stop();

			_log.Information("Successfully started Silo in {timeTaken:#.##}s (total).", _startupStopwatch.Elapsed.TotalSeconds);
		}

		private static async Task StopSilo(IAppInfo appInfo)
		{
			_log.Information("Stopping Silo '{siloName}'...", appInfo.Name);

			try
			{
				await _siloHost.StopAsync();
			}
			catch (Exception ex)
			{
				_log.Error(ex, "Stopping Silo failed '{siloName}'...", appInfo.Name);
			}

			_log.Information("Silo '{siloName}' shutdown.", appInfo.Name);

			SiloStopped.Set();
		}
	}
}