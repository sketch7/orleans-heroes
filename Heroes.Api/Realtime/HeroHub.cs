using System;
using System.Threading.Tasks;
using Heroes.Api.Realtime.Core;
using Heroes.Contracts.Grains.Heroes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Heroes.Api.Realtime
{
	public class HeroHub : Hub<IHeroHub>
	{
		private readonly string _source = $"{nameof(HeroHub)} ::";

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
		}

		public override async Task OnDisconnectedAsync(Exception ex)
		{
			_logger.Info("{hubName} User disconnected {connectionId}", _source, Context.ConnectionId);
			await Clients.All.Send($"{_source} {Context.ConnectionId} left");
		}

		public Task StreamUnsubscribe(string methodName, string id)
		{
			var key = $"{methodName}:{id}";
			return Task.CompletedTask;
			//if (Context.Connection.Metadata.TryGetValue(key, out object subscriptionObj))
			//{
			//	var subscription = (Subscription<Hero>)subscriptionObj;
			//	await subscription.Stream.UnsubscribeAsync();
			//	subscription.Subject.Dispose();
			//	Context.Connection.Metadata.Remove(key);
			//}
		}

		public Task<string> Echo(string message)
		{
			return Task.FromResult($"hello {message}");
		}
	}
}