using System;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.UserNotifications;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using SignalR.Orleans.Core;

namespace Heroes.Grains.UserNotifications
{
	[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
	public class UserNotificationGrain : AppGrain<UserNotificationState>, IUserNotificationGrain
	{
		private HubContext<IUserNotificationHub> _hubContext;

		public UserNotificationGrain(
			ILogger<HeroGrain> logger
		) : base(logger)
		{
		}

		public async Task Set(UserNotification item)
		{
			Logger.Info("updating grain state - {item}", item);
			State.UserNotification = item;
			await WriteStateAsync();
		}

		public Task<UserNotification> Get() => Task.FromResult(State.UserNotification);


		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();
			_hubContext = GrainFactory.GetHub<IUserNotificationHub>();
			
			var item = new UserNotification
			{
				MessageCount = 0
			};
			await Set(item);

			RegisterTimer(async x =>
			{
				State.UserNotification.MessageCount = RandomUtils.GenerateNumber(1, 100);
				await Set(State.UserNotification);

				var userNotification = new UserNotification
				{
					MessageCount = item.MessageCount
				};

				await _hubContext.User(PrimaryKey).SendSignalRMessage("Broadcast", userNotification);
			}, State, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
		}
	}
}