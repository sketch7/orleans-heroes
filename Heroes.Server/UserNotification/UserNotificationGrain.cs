using SignalR.Orleans.Core;

namespace Heroes.Server.UserNotification;

public sealed class UserNotificationGrain(
	ILogger<UserNotificationGrain> logger,
	[PersistentState("userNotification", OrleansConstants.GrainMemoryStorage)]
	IPersistentState<UserNotificationState> state
) : AppGrain<UserNotificationState>(logger, state), IUserNotificationGrain
{
	private HubContext<IUserNotificationHub>? _hubContext;

	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);

		_hubContext = GrainFactory.GetHub<IUserNotificationHub>();
		var hubUser = _hubContext.User(PrimaryKey);

		// Only initialize state when truly new (not previously persisted)
		if (State.UserNotification is null)
			await Set(new() { MessageCount = 0 });

		this.RegisterGrainTimer(async _ =>
		{
			var updated = new UserNotification { MessageCount = RandomUtils.GenerateNumber(1, 100) };
			await Set(updated);

			await hubUser.Send("Broadcast", updated);
		}, State, new() { DueTime = TimeSpan.FromSeconds(2), Period = TimeSpan.FromSeconds(3), Interleave = true });
	}

	public async Task Set(UserNotification item)
	{
		Logger.LogInformation("Updating grain state - {Item}", item);
		State.UserNotification = item;
		await WriteStateAsync();
	}

	public Task<UserNotification?> Get() => Task.FromResult(State.UserNotification);
}
