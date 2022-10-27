namespace Heroes.Contracts.Heroes;

public interface IHeroCollectionGrain : IGrainWithStringKey, IAppGrainContract
{
	Task<List<string>> GetAll(HeroRoleType? role = null);
}