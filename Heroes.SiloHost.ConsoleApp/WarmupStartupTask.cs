using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Heroes.Contracts.Grains;
using Orleans;
using Orleans.Runtime;

namespace Heroes.SiloHost.ConsoleApp
{
	public class WarmupStartupTask : IStartupTask
	{
		private readonly IGrainFactory _grainFactory;
		private readonly IExportLocatorScope _scope;

		public WarmupStartupTask(IGrainFactory grainFactory, IExportLocatorScope scope)
		{
			_grainFactory = grainFactory;
			_scope = scope;
		}

		public async Task Execute(CancellationToken cancellationToken)
		{

			var tenants = new List<string>
			{
				"lol",
				"hots"
			};

			foreach (var tenant in tenants)
			{
				var grain = _grainFactory.GetHeroCollectionGrain(tenant);
				await grain.Activate();

				var heroes = await grain.GetAll();
				foreach (var hero in heroes)
					_grainFactory.GetHeroGrain(tenant, hero.Key).InvokeOneWay(x => x.Activate());
			}
		}
	}
}
