namespace Heroes.Contracts.HeroCategories;

public interface IHeroCategoryCollectionGrain : IGrainWithStringKey, IAppGrainContract
{
	Task<List<string>> GetAll();
}