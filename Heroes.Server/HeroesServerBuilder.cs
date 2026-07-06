namespace Heroes.Server;

/// <summary>
/// Builder context for the <see cref="WebApplication"/>-based Heroes server host.
/// Mirrors the Arcane <c>ArcaneServerBuilder</c> pattern but without any Arcane dependency.
/// </summary>
public sealed class HeroesServerBuilder
{
	internal Action<IServiceCollection>? ConfigureServicesDelegate { get; private set; }
	internal Action<ISiloBuilder>? ConfigureOrleansDelegate { get; private set; }

	/// <summary>Gets the underlying <see cref="WebApplicationBuilder"/>.</summary>
	public WebApplicationBuilder Builder { get; }

	/// <summary>
	/// Resolved <see cref="IAppInfo"/> — populated by <see cref="HeroesServerHostExtensions.UseHeroesServer"/>
	/// after the config chain has been initialized.
	/// </summary>
	public IAppInfo AppInfo { get; internal set; } = null!;

	/// <summary>Shortcut to <see cref="Builder"/>.Configuration.</summary>
	public IConfiguration Configuration => Builder.Configuration;

	/// <inheritdoc cref="HeroesServerBuilder"/>
	public HeroesServerBuilder(WebApplicationBuilder builder) => Builder = builder;

	/// <summary>
	/// Configures DI services. Calling multiple times is additive.
	/// </summary>
	public HeroesServerBuilder ConfigureServices(Action<IServiceCollection> configure)
	{
		ConfigureServicesDelegate += configure;
		return this;
	}

	/// <summary>
	/// Configures the Orleans silo. Calling multiple times is additive.
	/// </summary>
	public HeroesServerBuilder ConfigureOrleans(Action<ISiloBuilder> configure)
	{
		ConfigureOrleansDelegate += configure;
		return this;
	}
}
