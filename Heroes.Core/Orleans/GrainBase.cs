using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Heroes.Core.Orleans
{
	/// <summary>
	/// Odin Grain implementation interface. e.g. Grain concrete should implement this.
	/// </summary>
	public interface IAppGrain : IAppGrainContract
	{
		/// <summary>
		/// Gets the primary key for the grain as string (independent of its original type).
		/// </summary>
		string PrimaryKey { get; }

		/// <summary>
		/// Gets the source type name e.g. 'AppConfigGrain'.
		/// </summary>
		string Source { get; }
	}

	/// <summary>
	/// Odin Grain public contract interface. e.g. Grain interface should implement this.
	/// </summary>
	public interface IAppGrainContract
	{
		/// <summary>
		/// Cause force activation in order for grain to be warmed up/preloaded.
		/// </summary>
		/// <returns></returns>
		[OneWay]
		Task Activate();
	}

	///// <summary>
	///// Extensions which applies to both Odin Grains, <see cref="AppGrain"/> and <see cref="AppGrain{TState}"/>.
	///// </summary>
	//public static class AppGrainExtensions
	//{
	//	private static readonly StringTokenParserFactory StringTokenParserFactory = new StringTokenParserFactory();

	//	/// <summary>
	//	/// Parses template key string into an object.
	//	/// </summary>
	//	/// <typeparam name="T">Type to cast key data to.</typeparam>
	//	/// <param name="grain"></param>
	//	/// <param name="template">Template pattern to parse e.g. '{brand}/{locale}/{id}'</param>
	//	/// <returns></returns>
	//	public static T ParseKey<T>(this IAppGrain grain, string template) where T : new()
	//		=> StringTokenParserFactory.Get(template)
	//		.Parse<T>(grain.PrimaryKey);
	//}

	public abstract class AppGrain<TState> : Grain<TState>, IAppGrain
		where TState : new()
	{
		protected readonly ILogger Logger;

		private string _primaryKey;
		public string PrimaryKey => _primaryKey ?? (_primaryKey = this.GetPrimaryKeyAny());

		public string Source { get; }

		protected AppGrain(ILogger logger)
		{
			Source = GetType().GetDemystifiedName();
			Logger = logger;
		}

		public virtual Task Activate() => Task.CompletedTask;

		public override Task OnActivateAsync()
		{
			if (!_primaryKey.IsNullOrEmpty())
				Logger.LogCritical("[{grain}] Grain PrimaryKey was set before activation! Make sure to null PrimaryKey on deactivation!", Source);

			Logger.LogInformation("[{grain}] activated for key: {grainPrimaryKey}", Source, PrimaryKey);
			return Task.CompletedTask;
		}

		public override Task OnDeactivateAsync()
		{
			Logger.LogInformation("[{grain}] deactivated for key: {grainPrimaryKey}", Source, PrimaryKey);
			return Task.CompletedTask;
		}

	}

	public abstract class AppGrain : Grain, IAppGrain
	{
		protected readonly ILogger Logger;

		private string _primaryKey;
		public string PrimaryKey => _primaryKey ?? (_primaryKey = this.GetPrimaryKeyAny());

		public string Source { get; }

		protected AppGrain(ILogger logger)
		{
			Source = GetType().GetDemystifiedName();
			Logger = logger;
		}

		public virtual Task Activate() => Task.CompletedTask;

		public override Task OnActivateAsync()
		{
			if (!_primaryKey.IsNullOrEmpty())
				Logger.LogCritical("[{grain}] Grain PrimaryKey was set before activation! Make sure to null PrimaryKey on deactivation!", Source);

			Logger.LogInformation("[{grain}] activated for key: {grainPrimaryKey}", Source, PrimaryKey);
			return Task.CompletedTask;
		}

		public override Task OnDeactivateAsync()
		{
			Logger.LogInformation("[{grain}] deactivated for key: {grainPrimaryKey}", Source, PrimaryKey);
			return Task.CompletedTask;
		}
	}
}
