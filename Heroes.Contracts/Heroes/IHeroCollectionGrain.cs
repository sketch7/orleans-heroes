using Heroes.Core.Orleans;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.Heroes
{
	public interface IHeroCollectionGrain : IGrainWithStringKey, IAppGrainContract
	{
		Task<List<string>> GetAll(HeroRoleType? role = null);
	}
}