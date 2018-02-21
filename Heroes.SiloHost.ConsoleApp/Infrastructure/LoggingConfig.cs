using Heroes.Core;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Heroes.SiloHost.ConsoleApp.Infrastructure
{
	public static class LoggingConfig
	{
		/// <summary>
		/// Configures simple logging which requires minimal configuration, which should be used early stages during bootup.
		/// </summary>
		/// <returns></returns>
		public static LoggerConfiguration ConfigureSimple()
		{
			var builder = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.Enrich.WithMachineName()
				.Enrich.WithDemystifiedStackTraces()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");
			return builder;
		}

		public static LoggerConfiguration Configure(IConfiguration config, IAppInfo appInfo)
		{
			var builder = ConfigureSimple();
			builder.ReadFrom.Configuration(config)
				.WithAppInfo(appInfo)
				;
			return builder;
		}

		// todo: move to core
		public static LoggerConfiguration WithAppInfo(this LoggerConfiguration builder, IAppInfo appInfo)
			=> builder.Enrich.WithProperty("appName", appInfo.ShortName)
				.Enrich.WithProperty("serviceType", appInfo.ServiceType)
				.Enrich.WithProperty("environment", appInfo.Environment)
				.Enrich.WithProperty("appVersion", appInfo.Version)
				.Enrich.WithProperty("gitCommit", appInfo.GitCommit);
	}
}
