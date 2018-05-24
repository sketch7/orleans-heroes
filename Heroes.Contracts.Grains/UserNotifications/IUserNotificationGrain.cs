using System.Threading.Tasks;
using Orleans;

namespace Heroes.Contracts.Grains.UserNotifications
{
	public interface IUserNotificationGrain : IGrainWithStringKey
	{
		Task Set(UserNotification item);
		Task<UserNotification> Get();
	}
}