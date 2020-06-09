using Heroes.Core.Orleans;
using Orleans;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Heroes.Contracts.HeroCategories
{
	public interface IHeroCategoryGrain : IGrainWithStringKey, IAppGrainContract
	{
		Task<HeroCategory> Get();
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class HeroCategory
	{
		protected string DebuggerDisplay => $"Key: '{Key}', Title: '{Title}'";

		public string Key { get; set; }
		public string Title { get; set; }
		public IList<string> Heroes { get; set; }
	}
}