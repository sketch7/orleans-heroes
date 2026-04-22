namespace Heroes.Server.Infrastructure;

public class ClientBuilderContext
{
	public string ClusterId => AppInfo.ClusterId;
	public string ServiceId => AppInfo.Name;

	public required ILogger Logger { get; init; }
	public required IAppInfo AppInfo { get; init; }
	public required IConfiguration Configuration { get; init; }
	public required Action<IClientBuilder> ConfigureClientBuilder { get; init; }
}