using Orleans.Concurrency;

namespace Heroes.Server.Infrastructure;

/// <summary>Grain public contract interface — grain interfaces should implement this.</summary>
public interface IAppGrainContract
{
	/// <summary>Cause force activation in order for the grain to be warmed up/preloaded.</summary>
	[OneWay]
	Task Activate();
}

/// <summary>Grain implementation interface — concrete grain classes implement this.</summary>
public interface IAppGrain : IAppGrainContract
{
	/// <summary>Gets the primary key for the grain as string (independent of its original type).</summary>
	string PrimaryKey { get; }

	/// <summary>Gets the source type name e.g. 'AppConfigGrain'.</summary>
	string Source { get; }
}

public abstract class AppGrain<TState> : Grain, IAppGrain
	where TState : new()
{
	protected readonly ILogger Logger;
	private readonly IPersistentState<TState> _stateContainer;

	private string? _primaryKey;

	/// <inheritdoc />
	public string PrimaryKey => _primaryKey ??= this.GetPrimaryKeyAny();

	/// <inheritdoc />
	public string Source { get; }

	/// <summary>Gets the current grain state.</summary>
	protected TState State => _stateContainer.State;

	/// <summary>Persists the current grain state.</summary>
	protected Task WriteStateAsync() => _stateContainer.WriteStateAsync();

	protected AppGrain(
		ILogger logger,
		IPersistentState<TState> state
	)
	{
		Source = GetType().GetDemystifiedName();
		Logger = logger;
		_stateContainer = state;
	}

	/// <inheritdoc />
	public virtual Task Activate() => Task.CompletedTask;

	public override Task OnActivateAsync(CancellationToken cancellationToken)
	{
		if (_primaryKey is not null)
			Logger.LogCritical("[{Grain}] Grain PrimaryKey was set before activation! Make sure to null PrimaryKey on deactivation!", Source);

		Logger.LogInformation("[{Grain}] activated for key: {GrainPrimaryKey}", Source, PrimaryKey);
		return Task.CompletedTask;
	}
}
