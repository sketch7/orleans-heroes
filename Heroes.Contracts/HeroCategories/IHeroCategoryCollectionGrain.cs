using Sketch7.Multitenancy.Orleans;

namespace Heroes.Contracts.HeroCategories;

public interface IHeroCategoryCollectionGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	Task<List<string>> GetAll();
}