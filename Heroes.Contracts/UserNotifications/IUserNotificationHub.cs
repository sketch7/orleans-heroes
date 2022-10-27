namespace Heroes.Contracts.UserNotifications;

public interface IUserNotificationHub
{
	Task Broadcast(UserNotification item);
	Task MessageCount(int count);
}