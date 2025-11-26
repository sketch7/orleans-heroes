using Heroes.Contracts;
using Heroes.Contracts.UserNotifications;
using Heroes.Core.Orleans;
using Heroes.Core.Utils;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

using SignalR.Orleans.Core;

namespace Heroes.Grains.UserNotifications;

[StorageProvider(ProviderName = OrleansConstants.GrainMemoryStorage)]
public class UserNotificationGrain : AppGrain<UserNotificationState>, IUserNotificationGrain
{
	private HubContext<IUserNotificationHub> _hubContext;

	public UserNotificationGrain(
		ILogger<UserNotificationGrain> logger
	) : base(logger)
	{
	}

	public async Task Set(UserNotification item)
	{
		Logger.LogInformation("updating grain state - {Item}", item);
		State.UserNotification = item;
		await WriteStateAsync();
	}

	public Task<UserNotification> Get() => Task.FromResult(State.UserNotification);


	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		_hubContext = GrainFactory.GetHub<IUserNotificationHub>();
		var hubUser = _hubContext.User(PrimaryKey);
		var item = new UserNotification
		{
			MessageCount = 0
		};
		await Set(item);

		this.RegisterGrainTimer(async x =>
		{
			State.UserNotification.MessageCount = RandomUtils.GenerateNumber(1, 100);
			await Set(State.UserNotification);

			var userNotification = new UserNotification
			{
				MessageCount = item.MessageCount
			};

			await hubUser.Send("Broadcast", userNotification);
		}, State, new GrainTimerCreationOptions { DueTime = TimeSpan.FromSeconds(2), Period = TimeSpan.FromSeconds(3), Interleave = true });
	}
}