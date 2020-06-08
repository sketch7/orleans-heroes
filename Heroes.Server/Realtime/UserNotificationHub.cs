using Heroes.Contracts.UserNotifications;
using Microsoft.AspNetCore.SignalR;

namespace Heroes.Server.Realtime
{
	public class UserNotificationHub : Hub<IUserNotificationHub>
	{
	}
}