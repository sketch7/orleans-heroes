using Orleans;

namespace Heroes.Server.Infrastructure;

public class ClientBuilderContext
{
	public string ClusterId => AppInfo.ClusterId;
	public string ServiceId => AppInfo.Name;

	public ILogger Logger { get; set; }
	public IAppInfo AppInfo { get; set; }
	public IConfiguration Configuration { get; set; }
	public Action<IClientBuilder> ConfigureClientBuilder { get; set; }
}