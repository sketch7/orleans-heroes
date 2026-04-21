using Heroes.Contracts;

namespace Heroes.Server;

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
		using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
		var ct = linked.Token;

		foreach (var tenant in _appTenantRegistry.GetAll())
		{
			if (ct.IsCancellationRequested)
				break;

			try
			{
				var grain = _grainFactory.GetHeroCollectionGrain(tenant.Key);
				await grain.Activate();

				var heroes = await grain.GetAll();
				foreach (var heroKey in heroes)
				{
					if (ct.IsCancellationRequested)
						break;

					try
					{
						await _grainFactory.GetHeroGrain(tenant.Key, heroKey).Activate();
					}
					catch (Exception ex)
					{
						// Log but don't fail the entire warmup if one hero fails
						Console.WriteLine($"Failed to activate hero {heroKey} for tenant {tenant.Key}: {ex.Message}");
					}
				}
			}
			catch (Exception ex)
			{
				// Log but don't fail the entire warmup if one tenant fails
				Console.WriteLine($"Failed to warmup tenant {tenant.Key}: {ex.Message}");
			}
		}
	}
}