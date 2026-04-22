namespace Heroes.Server.UserNotification;

public interface IUserNotificationGrain : IGrainWithStringKey
{
	Task Set(UserNotification item);
	Task<UserNotification> Get();
}

public interface IUserNotificationHub
{
	Task Broadcast(UserNotification item);
	Task MessageCount(int count);
}

[GenerateSerializer]
public sealed class UserNotificationState
{
	[Id(0)]
	public UserNotification UserNotification { get; set; }
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class UserNotification
{
	private string DebuggerDisplay => $"MessageCount: '{MessageCount}'";

	[Id(0)]
	public int MessageCount { get; set; }

	public override string ToString() => DebuggerDisplay;
}
