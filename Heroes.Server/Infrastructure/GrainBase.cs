using Microsoft.Extensions.Logging;
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

/// <summary>Extensions which apply to <see cref="AppGrain{TState}"/>.</summary>
public static class AppGrainExtensions
{
	private static readonly StringTokenParserFactory StringTokenParserFactory = new();

	/// <summary>Parses a template key string into an object.</summary>
	/// <typeparam name="T">Type to cast key data to.</typeparam>
	/// <param name="grain">The grain instance.</param>
	/// <param name="template">Template pattern to parse e.g. '{brand}/{locale}/{id}'</param>
	public static T ParseKey<T>(this IAppGrain grain, string template) where T : new()
		=> StringTokenParserFactory.Get(template).Parse<T>(grain.PrimaryKey);
}

public abstract class AppGrain<TState> : Grain<TState>, IAppGrain
	where TState : new()
{
	protected readonly ILogger Logger;

	private string _primaryKey;

	/// <inheritdoc />
	public string PrimaryKey => _primaryKey ??= this.GetPrimaryKeyAny();

	/// <inheritdoc />
	public string Source { get; }

	protected AppGrain(ILogger logger)
	{
		Source = GetType().GetDemystifiedName();
		Logger = logger;
	}

	/// <inheritdoc />
	public virtual Task Activate() => Task.CompletedTask;

	public override Task OnActivateAsync(CancellationToken cancellationToken)
	{
		if (!_primaryKey.IsNullOrEmpty())
			Logger.LogCritical("[{Grain}] Grain PrimaryKey was set before activation! Make sure to null PrimaryKey on deactivation!", Source);

		Logger.LogInformation("[{Grain}] activated for key: {GrainPrimaryKey}", Source, PrimaryKey);
		return Task.CompletedTask;
	}
}
