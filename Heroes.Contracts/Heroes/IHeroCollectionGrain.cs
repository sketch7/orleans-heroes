using Sketch7.Multitenancy.Orleans;

namespace Heroes.Contracts.Heroes;

public interface IHeroCollectionGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	Task<List<string>> GetAll(HeroRoleType? role = null);
}