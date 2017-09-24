using System.Threading.Tasks;

namespace Heroes.Contracts.Grains.Core
{
	public interface IWarmUpClient
	{
		Task Initialize();
	}
}