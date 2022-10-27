namespace Heroes.Contracts.HeroCategories;

public interface IHeroCategoryGrain : IGrainWithStringKey, IAppGrainContract
{
	Task<HeroCategory> Get();
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeroCategory
{
	protected string DebuggerDisplay => $"Id: '{Id}', Title: '{Title}'";

	public string Id { get; set; }
	public string Title { get; set; }
	public IList<string> Heroes { get; set; }
}