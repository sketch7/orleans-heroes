using Heroes.Core;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.SiloHost.ConsoleApp
{
	public class LoggingIncomingCallFilter : IIncomingGrainCallFilter
	{
		private readonly ILogger _logger;

		public LoggingIncomingCallFilter(ILogger<LoggingIncomingCallFilter> logger)
		{
			_logger = logger;
		}

		public async Task Invoke(IIncomingGrainCallContext context)
		{
			var grainType = context.Grain.GetType();
			var grainName = grainType.GetDemystifiedName();
			var shouldHaveDetailedTrace = grainType.Namespace.Contains("Heroes"); // todo: Create log filter mechanism

			if (!shouldHaveDetailedTrace)
			{
				await context.Invoke();
				return;
			}

			string primaryKey = null;
			if (context.Grain is Grain grain)
				primaryKey = grain.GetPrimaryKeyAny();

			var stopwatch = Stopwatch.StartNew();

			try
			{
				await context.Invoke();
				stopwatch.Stop();

				//if (stopwatch.Elapsed > WarnThreshold)
				//{
				_logger.LogDebug(
					"Executed grain method {grain}.{grainMethod} ({primaryKey}) in {duration:n0}ms",
					grainName,
					context.ImplementationMethod.Name,
					primaryKey,
					stopwatch.ElapsedMilliseconds
				);
				//}
			}
			catch (Exception ex)
			{
				_logger.LogError(
						ex,
						"Execution failed for grain method {grain}.{grainMethod} ({primaryKey}) in {duration:n0}ms.",
						primaryKey,
						grainName,
						context.ImplementationMethod.Name,
						stopwatch.ElapsedMilliseconds
						)
					;
				throw;
			}
		}
	}


	public class LoggingOutgoingCallFilter : IOutgoingGrainCallFilter
	{
		private readonly ILogger _logger;

		public LoggingOutgoingCallFilter(ILogger<LoggingOutgoingCallFilter> logger)
		{
			_logger = logger;
		}

		public async Task Invoke(IOutgoingGrainCallContext context)
		{
			var grainType = context.Grain.GetType();
			var grainName = grainType.GetDemystifiedName();
			var shouldHaveDetailedTrace = grainType.Namespace.Contains("Heroes"); // todo: Create log filter mechanism

			if (!shouldHaveDetailedTrace)
			{
				await context.Invoke();
				return;
			}

			string primaryKey = null;
			if (context.Grain is Grain grain)
				primaryKey = grain.GetPrimaryKeyAny();

			var stopwatch = Stopwatch.StartNew();

			try
			{
				await context.Invoke();
				stopwatch.Stop();

				//if (stopwatch.Elapsed > WarnThreshold)
				//{
				_logger.LogDebug(
					"Invoking grain method {grain}.{grainMethod} ({primaryKey}) in {duration:n0}ms",
					grainName,
					context.InterfaceMethod,
					primaryKey,
					stopwatch.ElapsedMilliseconds
				);
				//}
			}
			catch (Exception ex)
			{
				_logger.LogError(
						ex,
						"Execution failed for grain method {grain}.{grainMethod} ({primaryKey}) in {duration:n0}ms.",
						primaryKey,
						grainName,
						context.InterfaceMethod,
						stopwatch.ElapsedMilliseconds
					)
					;
				throw;
			}
		}
	}
}
