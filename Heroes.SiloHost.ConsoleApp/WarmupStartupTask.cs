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
			foreach (var tenant in Tenants.All)
			{
				var grain = _grainFactory.GetHeroCollectionGrain(tenant.Key);
				await grain.Activate();

				var heroes = await grain.GetAll();
				foreach (var hero in heroes)
					_grainFactory.GetHeroGrain(tenant.Key, hero.Key).InvokeOneWay(x => x.Activate());
			}
		}
	}
}
