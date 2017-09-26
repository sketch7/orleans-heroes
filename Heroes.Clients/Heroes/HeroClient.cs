using Heroes.Contracts.Grains;
using Heroes.Contracts.Grains.Heroes;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			var grain = _clusterClient.GetHeroGrain(key);
			return grain.Get();
		}

		public Task<List<Hero>> GetAll(HeroRoleType? role = null)
		{
			var grain = _clusterClient.GetHeroCollectionGrain();
			return grain.GetAll(role);
		}

		public Task Set(Hero hero)
		{
			return Set(new List<Hero> { hero });
		}

		public Task Set(List<Hero> heroes)
		{
			var grain = _clusterClient.GetHeroCollectionGrain();
			return grain.Set(heroes);
		}
	}
}