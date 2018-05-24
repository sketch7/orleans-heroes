using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.UserNotifications
{
	public interface IUserNotificationHub
	{
		Task Broadcast(UserNotification item);
		Task MessageCount(int count);
	}
}