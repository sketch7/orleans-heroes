using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heroes.Contracts.HeroCategories
{
	public interface IHeroCategoryGrainClient
	{
		Task<HeroCategory> Get(string key);
		Task<List<HeroCategory>> GetAll();
	}
}