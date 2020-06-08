using Heroes.Contracts.Heroes;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heroes.Contracts;

namespace Heroes.GrainClients.Heroes
{
	public class HeroGrainClient : IHeroGrainClient
	{
		private readonly IGrainFactory _grainFactory;

		public HeroGrainClient(
			IGrainFactory grainFactory
		)
		{
			_grainFactory = grainFactory;
		}

		public Task<Hero> Get(string key)
		{
			// todo: get tenant from context
			var grain = _grainFactory.GetHeroGrain("lol", key);
			return grain.Get();
		}

		public Task<List<Hero>> GetAll(HeroRoleType? role = null)
		{
			var grain = _grainFactory.GetHeroCollectionGrain("lol");
			return grain.GetAll(role);
		}
	}
}