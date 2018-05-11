using System;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.UserNotifications;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using SignalR.Orleans.Core;

namespace Heroes.Grains.UserNotifications
{
	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class UserNotificationGrain : Grain<UserNotificationState>, IUserNotificationGrain
	{
		private const string Source = nameof(UserNotificationGrain);
		private readonly ILogger<HeroGrain> _logger;
		private readonly Random _random = new Random();
		private HubContext<IUserNotificationHub> _hubContext;

		public UserNotificationGrain(
			ILogger<HeroGrain> logger
		)
		{
			_logger = logger;
		}

		public async Task Set(UserNotification item)
		{
			_logger.LogInformation("updating grain state - {item}", item);
			State.UserNotification = item;
			await WriteStateAsync();
		}

		public Task<UserNotification> Get() => Task.FromResult(State.UserNotification);


		public override async Task OnActivateAsync()
		{
			_hubContext = GrainFactory.GetHub<IUserNotificationHub>();

			_logger.LogInformation("{Source} :: OnActivateAsync PK {PK}", Source, this.GetPrimaryKeyString());
			var item = new UserNotification
			{
				MessageCount = 0
			};
			await Set(item);

			RegisterTimer(async x =>
			{
				State.UserNotification.MessageCount = _random.Next(100);
				await Set(State.UserNotification);

				var userNotification = new UserNotification
				{
					MessageCount = item.MessageCount
				};

				await _hubContext.User(this.GetPrimaryKeyString()).SendSignalRMessage("Broadcast", userNotification);
			}, State, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
		}

		public override Task OnDeactivateAsync()
		{
			_logger.LogInformation("{Source} :: OnDeactivateAsync PK {PK}", Source, this.GetPrimaryKeyString());
			return Task.CompletedTask;
		}
	}
}