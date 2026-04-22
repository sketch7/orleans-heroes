using Microsoft.AspNetCore.SignalR;
using Orleans.Streams;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Heroes.Server.Hero;

file sealed class Subscription<T>
{
	public required StreamSubscriptionHandle<T> Stream { get; init; }
	public required Subject<T> Subject { get; init; }
}

public sealed class HeroHub : Hub<IHeroHub>
{
	private const string HeroStreamProviderKey = "hero-StreamProvider";

	private readonly IClusterClient _clusterClient;
	private readonly ILogger _logger;

	public HeroHub(
		IClusterClient clusterClient,
		ILogger<HeroHub> logger
	)
	{
		_clusterClient = clusterClient;
		_logger = logger;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.LogInformation("User connected {ConnectionId}", Context.ConnectionId);

		await Clients.All.Send($"{Context.ConnectionId} joined");

		if (Context.User?.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(Context.User.Identity.Name))
		{
			await Clients.User(Context.User.Identity.Name)
				.Send($"logged in user => {Context.User.Identity.Name} -> ConnectionId: {Context.ConnectionId}");
		}

		var streamProvider = _clusterClient.GetStreamProvider(OrleansConstants.STREAM_PROVIDER);
		Context.Items.Add(HeroStreamProviderKey, streamProvider);
	}

	public override async Task OnDisconnectedAsync(Exception? ex)
	{
		_logger.LogInformation("User disconnected {ConnectionId}", Context.ConnectionId);
		await Clients.All.Send($"{Context.ConnectionId} left");
	}

	public ChannelReader<Hero> GetUpdates(string id)
	{
		if (!Context.Items.TryGetValue(HeroStreamProviderKey, out var streamProviderObj) || streamProviderObj is not IStreamProvider streamProvider)
			throw new InvalidOperationException("Stream provider not available. Ensure OnConnectedAsync completed successfully.");

		var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream.ToString(), $"hero:{id}");
		var heroSubject = new Subject<Hero>();

		Task.Run(async () =>
		{
			var heroStream = await stream.SubscribeAsync(async (action, st) =>
			{
				_logger.LogInformation("Stream [hero.health] triggered {Action}", action);
				await Clients.All.Send("msg ->");
				heroSubject.OnNext(action);
			});
			Context.Items.Add($"{nameof(GetUpdates)}:{id}", new Subscription<Hero>
			{
				Stream = heroStream,
				Subject = heroSubject
			});
		});

		return heroSubject.AsObservable().AsChannelReader();
	}

	public async Task AddToGroup(string name)
		=> await Groups.AddToGroupAsync(Context.ConnectionId, name);

	public async Task AddToGroups(HashSet<string> groups)
	{
		foreach (var group in groups)
			await Groups.AddToGroupAsync(Context.ConnectionId, group);
	}

	public async Task StreamUnsubscribe(string methodName, string id)
	{
		var key = $"{methodName}:{id}";
		if (!Context.Items.TryGetValue(key, out var subscriptionObj) || subscriptionObj is not Subscription<Hero> subscription)
			return;

		await subscription.Stream.UnsubscribeAsync();
		subscription.Subject.Dispose();
		Context.Items.Remove(key);
	}

	public Task<string> Echo(string message)
		=> Task.FromResult($"hello {message}");

	public Task<string> EchoGroup(HashSet<string> message)
		=> Task.FromResult($"hello {message}");
}
