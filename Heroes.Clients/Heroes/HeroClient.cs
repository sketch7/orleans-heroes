using Heroes.Contracts.Grains;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heroes.Contracts.Grains.Heroes;

namespace Heroes.Clients.Heroes
{
	public class HeroClient : IHeroClient
	{
		private readonly IClusterClient _clusterClient;

		public HeroClient(IClusterClient clusterClient)
		{
			_clusterClient = clusterClient;
		}

		public Task<Hero> Get(string key)
		{
			var grain = _clusterClient.GetGrain<IHeroGrain>(key);
			return grain.Get();
		}

		public Task<List<Hero>> GetAll(HeroRoleType? role = null)
		{
			var grain = _clusterClient.GetGrain<IHeroCollectionGrain>(0);
			return grain.GetAll(role);
		}
	}
}