namespace Heroes.Contracts.UserNotifications;

[GenerateSerializer]
public class UserNotificationState
{
	[Id(0)]
	public UserNotification UserNotification { get; set; }
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public class UserNotification
{
	protected string DebuggerDisplay => $"MessageCount: '{MessageCount}'";
	[Id(0)]
	public int MessageCount { get; set; }

	public override string ToString() => DebuggerDisplay;
}