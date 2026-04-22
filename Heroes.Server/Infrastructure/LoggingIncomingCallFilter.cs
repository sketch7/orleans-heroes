namespace Heroes.Server.Infrastructure;

public sealed class LoggingIncomingCallFilter : IIncomingGrainCallFilter
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
		var shouldHaveDetailedTrace = grainType.Namespace?.Contains("Heroes", StringComparison.Ordinal) == true;

		if (!shouldHaveDetailedTrace)
		{
			await context.Invoke();
			return;
		}

		string? primaryKey = null;
		if (context.Grain is Grain grain)
			primaryKey = grain.GetPrimaryKeyAny();

		var stopwatch = Stopwatch.StartNew();

		try
		{
			await context.Invoke();
			stopwatch.Stop();

			_logger.LogDebug(
				"Executed grain method {Grain}.{GrainMethod} ({PrimaryKey}) in {Duration:n0}ms",
				grainName,
				context.ImplementationMethod.Name,
				primaryKey,
				stopwatch.ElapsedMilliseconds
			);
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Execution failed for grain method {Grain}.{GrainMethod} ({PrimaryKey}) in {Duration:n0}ms.",
				grainName,
				context.ImplementationMethod.Name,
				primaryKey,
				stopwatch.ElapsedMilliseconds
			);
			throw;
		}
	}
}
