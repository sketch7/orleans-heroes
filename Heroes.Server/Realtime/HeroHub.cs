using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core.Extensions;
using Heroes.Server.Realtime.Core;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using SignalR.Orleans;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Heroes.Server.Realtime;

public class HeroHub : Hub<IHeroHub>
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
		_logger.LogInformation("User connected {connectionId}", Context.ConnectionId);

		await Clients.All.Send($"{Context.ConnectionId} joined");

		if (Context.User.Identity.IsAuthenticated)
		{
			var loggedInUser = Clients.User(Context.User.Identity.Name);
			await loggedInUser.Send($"logged in user => {Context.User.Identity.Name} -> ConnectionId: {Context.ConnectionId}");
		}

		var streamProvider = _clusterClient.GetStreamProvider(Constants.STREAM_PROVIDER);
		Context.Items.Add(HeroStreamProviderKey, streamProvider);
	}

	public override async Task OnDisconnectedAsync(Exception ex)
	{
		_logger.Info("User disconnected {connectionId}", Context.ConnectionId);
		await Clients.All.Send($"{Context.ConnectionId} left");
	}

	public ChannelReader<Hero> GetUpdates(string id)
	{
		// todo: this method need to be fixed
		Context.Items.TryGetValue(HeroStreamProviderKey, out var streamProviderObj);
		var streamProvider = (IStreamProvider)streamProviderObj;
		var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{id}");
		var heroSubject = new Subject<Hero>();

		Task.Run(async () =>
		{
			var heroStream = await stream.SubscribeAsync(async (action, st) =>
			{
				_logger.Info("Stream [hero.health] triggered {action}", action);
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

	public async Task StreamUnsubscribe(string methodName, string id)
	{
		var key = $"{methodName}:{id}";
		if (Context.Items.TryGetValue(key, out var subscriptionObj))
		{
			var subscription = (Subscription<Hero>)subscriptionObj;
			await subscription.Stream.UnsubscribeAsync();
			subscription.Subject.Dispose();
			Context.Items.Remove(key);
		}
	}

	public Task<string> Echo(string message)
		=> Task.FromResult($"hello {message}");
}