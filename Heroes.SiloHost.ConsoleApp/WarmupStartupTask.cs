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
		private readonly IAppTenantRegistry _appTenantRegistry;

		public WarmupStartupTask(
			IGrainFactory grainFactory,
			IAppTenantRegistry appTenantRegistry
		)
		{
			_grainFactory = grainFactory;
			_appTenantRegistry = appTenantRegistry;
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			foreach (var tenant in _appTenantRegistry.GetAll())
			{
				var grain = _grainFactory.GetHeroCollectionGrain(tenant.Key);
				await grain.Activate();

				var heroes = await grain.GetAll();
				foreach (var hero in heroes)
					await _grainFactory.GetHeroGrain(tenant.Key, hero.Key).Activate();
			}
		}
	}
}
