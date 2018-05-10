using System.Threading;
using System.Threading.Tasks;
using Heroes.Contracts.Grains;
using Orleans;
using Orleans.Runtime;

namespace Heroes.SiloHost.ConsoleApp
{
	public class WarmupStartupTask : IStartupTask
	{
		private readonly IGrainFactory _grainFactory;

		public WarmupStartupTask(IGrainFactory grainFactory)
		{
			_grainFactory = grainFactory;
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			var grain = _grainFactory.GetHeroCollectionGrain();
			await grain.Activate();

			var heroes = await grain.GetAll();
			foreach (var hero in heroes)
				_grainFactory.GetHeroGrain(hero.Key).InvokeOneWay(x => x.Activate());
		}
	}
}
