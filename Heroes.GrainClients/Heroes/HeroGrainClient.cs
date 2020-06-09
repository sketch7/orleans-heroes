using Heroes.Contracts;
using Heroes.Contracts.Heroes;
using Heroes.Core;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.GrainClients.Heroes
{
	public class HeroGrainClient : IHeroGrainClient
	{
		// todo: get tenant from context
		private const string Tenant = "lol";
		private readonly IGrainFactory _grainFactory;

		public HeroGrainClient(
			IGrainFactory grainFactory
		)
		{
			_grainFactory = grainFactory;
		}

		public Task<Hero> Get(string key)
		{
			var grain = _grainFactory.GetHeroGrain(Tenant, key);
			return grain.Get();
		}

		public async Task<List<Hero>> GetAllByRefs(ICollection<string> keys)
		{
			if (keys?.Count == 0)
				return null;

			var heroes = await keys.SelectAsync(key => _grainFactory.GetHeroGrain(Tenant, key).Get());
			return heroes.ToList();
		}

		public Task<List<Hero>> GetAll(HeroRoleType? role = null)
		{
			var grain = _grainFactory.GetHeroCollectionGrain(Tenant);
			return grain.GetAll(role);
		}
	}
}