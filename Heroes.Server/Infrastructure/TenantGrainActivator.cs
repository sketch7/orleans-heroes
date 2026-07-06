using Heroes.Contracts;
using Orleans;
using Orleans.Runtime;

namespace Heroes.Server.Infrastructure;

/// <summary>
/// <see cref="IGrainActivator"/> that activates grains using the standard <see cref="IServiceProvider"/>.
/// </summary>
public class TenantGrainActivator : IGrainActivator
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<TenantGrainActivator> _logger;

	public TenantGrainActivator(
		IServiceProvider serviceProvider,
		ILogger<TenantGrainActivator> logger
	)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public object CreateInstance(IGrainContext context)
	{
		var grainType = context.GrainInstance?.GetType() ?? typeof(object);
		return ActivatorUtilities.CreateInstance(_serviceProvider, grainType, context);
	}

	public ValueTask DisposeInstance(IGrainContext context, object grain)
	{
		if (grain is IDisposable disposable)
			disposable.Dispose();
		return ValueTask.CompletedTask;
	}
}
