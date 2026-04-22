using Orleans.Concurrency;
using Sketch7.Multitenancy.Orleans;

namespace Heroes.Server.HeroCategory;

[Alias("IHeroCategoryGrain")]
public interface IHeroCategoryGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<HeroCategoryModel?> Get();
}

[Alias("IHeroCategoryCollectionGrain")]
public interface IHeroCategoryCollectionGrain : IGrainWithStringKey, IAppGrainContract, ITenantGrain
{
	[AlwaysInterleave]
	[return: Immutable]
	Task<List<string>> GetAll();
}

[GenerateSerializer]
public sealed record HeroCategoryModel
{
	[Id(0)]
	public required string Id { get; init; }
	[Id(1)]
	public required string Title { get; init; }
	[Id(2)]
	public IReadOnlyList<string> Heroes { get; init; } = [];
}
