using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Heroes
{
	public interface IHeroHub
	{
		Task Send(string message);
		Task Broadcast(Hero hero);
		Task StreamUnsubscribe(string methodName, string id);
	}
}