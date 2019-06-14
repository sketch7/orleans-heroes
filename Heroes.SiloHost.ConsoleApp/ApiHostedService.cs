﻿using Grace.AspNetCore.Hosting;
using Grace.DependencyInjection;
using Heroes.Api;
using Heroes.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Heroes.SiloHost.ConsoleApp
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
			IMembershipOracle oracle,
			IConfiguration configuration,
			IExportLocatorScope exportScope,
			IAppInfo appInfo,
			ILogger<ApiHostedService> logger
		)
		{
			_appInfo = appInfo;
			_logger = logger;
			logger.LogInformation("Initializing api {appName} ({version}) [{env}]...",
				appInfo.Name, appInfo.Version, appInfo.Environment);

			_host = WebHost.CreateDefaultBuilder()
				.UseGrace(new InjectionScopeConfiguration
				{
					Behaviors = { AllowInstanceAndFactoryToReturnNull = true }
				})
				.UseSerilog()
				.UseConfiguration(configuration)
				.ConfigureServices(services =>
				{
					services.AddSingleton(appInfo);
					services.AddSingleton(client);
					services.AddSingleton(exportScope);
				})
				.UseStartup<ApiStartup>()
				.UseUrls($"http://*:{options.Value.Port}")
				.Build();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("App started successfully {appName} ({version}) [{env}]...",
				_appInfo.Name, _appInfo.Version, _appInfo.Environment);
			return _host.StartAsync(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);


	}
}
