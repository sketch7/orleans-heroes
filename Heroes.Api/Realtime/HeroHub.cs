using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;
using System.Threading.Tasks;
using Heroes.Api.Realtime.Core;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Core.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using SignalR.Orleans;

namespace Heroes.Api.Realtime
{
	public class HeroHub : Hub<IHeroHub>
	{
		private readonly string _source = $"{nameof(HeroHub)} ::";
		private const string HeroStreamProviderKey = "hero-StreamProvider";

		private readonly IClusterClient _clusterClient;
		private readonly ILogger<HeroHub> _logger;

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
			_logger.LogInformation("{hubName} User connected {connectionId}", _source, Context.ConnectionId);

			await Clients.All.Send($"{_source} {Context.ConnectionId} joined");

			if (Context.User.Identity.IsAuthenticated)
			{
				var loggedInUser = Clients.User(Context.User.Identity.Name);
				await loggedInUser.Send(
					$"{_source} logged in user => {Context.User.Identity.Name} -> ConnectionId: {Context.ConnectionId}");
			}

			var streamProvider = _clusterClient.GetStreamProvider(Constants.STREAM_PROVIDER);
			Context.Items.Add(HeroStreamProviderKey, streamProvider);
		}

		public override async Task OnDisconnectedAsync(Exception ex)
		{
			_logger.Info("{hubName} User disconnected {connectionId}", _source, Context.ConnectionId);
			await Clients.All.Send($"{_source} {Context.ConnectionId} left");
		}

		public ChannelReader<Hero> GetUpdates(string id)
		{
			//TODO: this method need to be fixed.
			Context.Items.TryGetValue(HeroStreamProviderKey, out object streamProviderObj);
			var streamProvider = (IStreamProvider)streamProviderObj;
			var stream = streamProvider.GetStream<Hero>(StreamConstants.HeroStream, $"hero:{id}");
			var heroSubject = new Subject<Hero>();

			Task.Run(async () =>
			{
				var heroStream = await stream.SubscribeAsync(
					async (action, st) =>
					{
						_logger.Info("{hubName} Stream [hero.health] triggered {action}", _source, action);
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

		public async Task StreamUnsubscribe(string methodName, string id)
		{
			var key = $"{methodName}:{id}";
			if (Context.Items.TryGetValue(key, out object subscriptionObj))
			{
				var subscription = (Subscription<Hero>)subscriptionObj;
				await subscription.Stream.UnsubscribeAsync();
				subscription.Subject.Dispose();
				Context.Items.Remove(key);
			}
		}

		public Task<string> Echo(string message)
		{
			return Task.FromResult($"hello {message}");
		}
	}
}