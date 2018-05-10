using System;
using System.Threading.Tasks;
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
	}
}