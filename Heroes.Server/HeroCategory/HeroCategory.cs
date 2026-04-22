using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.HeroCategory;

public interface IHeroCategoryGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	Task<HeroCategoryModel> Get();
}

public interface IHeroCategoryCollectionGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	Task<List<string>> GetAll();
}

[GenerateSerializer, DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeroCategoryModel
{
	protected string DebuggerDisplay => $"Id: '{Id}', Title: '{Title}'";

	[Id(0)]
	public string Id { get; set; }
	[Id(1)]
	public string Title { get; set; }
	[Id(2)]
	public IList<string> Heroes { get; set; }
}
