using Heroes.Contracts.Grains.UserNotifications;
using Microsoft.AspNetCore.SignalR;

namespace Heroes.Api.Realtime
{
	public class UserNotificationHub : Hub<IUserNotificationHub>
	{
	}
}