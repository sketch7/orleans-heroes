using Orleans.Concurrency;

namespace Heroes.Server.UserNotification;

[Alias("IUserNotificationGrain")]
public interface IUserNotificationGrain : IGrainWithStringKey
{
	Task Set([Immutable] UserNotification item);

	[AlwaysInterleave]
	[return: Immutable]
	Task<UserNotification?> Get();
}

[Alias("IUserNotificationHub")]
public interface IUserNotificationHub
{
	Task Broadcast(UserNotification item);
	Task MessageCount(int count);
}

[GenerateSerializer]
public sealed class UserNotificationState
{
	[Id(0)]
	public UserNotification? UserNotification { get; set; }
}

[GenerateSerializer]
public sealed record UserNotification
{
	[Id(0)]
	public int MessageCount { get; init; }
}
