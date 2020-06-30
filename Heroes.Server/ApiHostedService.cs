using Grace.AspNetCore.Hosting;
using Grace.DependencyInjection;
using Heroes.Core;
using Heroes.Core.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Serilog;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Heroes.Server
{
	public class ApiHostedServiceOptions
	{
		//public string PathString { get; set; } = "/health";
		public int Port { get; set; } = 8880;
	}

	public class ApiHostedService : IHostedService
	{
		private readonly IAppInfo _appInfo;
		private readonly ILogger _logger;
		private readonly IWebHost _host;

		public ApiHostedService(
			IOptions<ApiHostedServiceOptions> options,
			IClusterClient client,
			IGrainFactory grainFactory,
			IConfiguration configuration,
			IExportLocatorScope exportScope,
			IAppInfo appInfo,
			ILogger<ApiHostedService> logger
		)
		{
			_appInfo = appInfo;
			_logger = logger;
			logger.LogInformation("Initializing api {appName} ({version}) [{env}] on port {apiPort}...",
				appInfo.Name, appInfo.Version, appInfo.Environment, options.Value.Port);
			ConsoleTitleBuilder.Append(() => $"(Api port: {options.Value.Port} | pid: {Process.GetCurrentProcess().Id})");

			_host = WebHost.CreateDefaultBuilder()
				.UseGrace(new InjectionScopeConfiguration
				{
					Behaviors = { AllowInstanceAndFactoryToReturnNull = true }
				})
				.UseSerilog()
				.UseConfiguration(configuration)
				.ConfigureAppConfiguration(cfg =>
				{
					cfg.Sources.Clear();
					cfg.AddConfiguration(configuration);
				})
				.ConfigureServices(services =>
				{
					services.AddSingleton(appInfo);
					services.AddSingleton(client);
					services.AddSingleton(grainFactory);
					services.AddSingleton(exportScope);
				})
				.UseStartup<ApiStartup>()
				.UseUrls($"http://*:{options.Value.Port}")
				.Build();
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("App started successfully {appName} ({version}) [{env}]",
				_appInfo.Name, _appInfo.Version, _appInfo.Environment);
			await _host.StartAsync(cancellationToken);
			ConsoleTitleBuilder.Append("- Api status: running 🚀");
		}

		public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
	}
}
