using Heroes.Core.Orleans;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.HeroCategories
{
	public interface IHeroCategoryCollectionGrain : IGrainWithStringKey, IAppGrainContract
	{
		Task<List<string>> GetAll();
	}
}