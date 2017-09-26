using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Core;
using Heroes.Contracts.Grains.Mocks;
using Orleans;
using System.Threading.Tasks;

namespace Heroes.Clients
{
	public class WarmUpClient : IWarmUpClient
	{
		private readonly IClusterClient _clusterClient;

		public WarmUpClient(IClusterClient clusterClient)
		{
			_clusterClient = clusterClient;
		}
		public Task Initialize()
		{
			var heroCollectionGrain = _clusterClient.GetHeroCollectionGrain();
			return heroCollectionGrain.Set(MockDataService.GetHeroes());
		}
	}
}