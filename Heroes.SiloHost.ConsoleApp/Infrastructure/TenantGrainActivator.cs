using System;
using System.Text.RegularExpressions;
using Grace.DependencyInjection;
using Heroes.Contracts.Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;

namespace Heroes.SiloHost.ConsoleApp.Infrastructure
{
	/// <summary>
	/// <see cref="IGrainActivator"/> that uses type activation to create grains.
	/// </summary>
	public class TenantGrainActivator : DefaultGrainActivator
	{
		private readonly Regex _regex = new Regex(@"tenant\/(.*)", RegexOptions.Compiled);

		public TenantGrainActivator(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override object Create(IGrainActivationContext context)
		{
			var scope = context.ActivationServices.GetRequiredService<IExportLocatorScope>();


			//var t = RequestContext.Get("tenant");

			var matches = _regex.Match(context.GrainIdentity.PrimaryKeyString ?? string.Empty);
			if (context.GrainIdentity.PrimaryKeyString != null && matches.Success)
			{
				var tenantContext = new AppTenant
				{
					Key = matches.Groups[1].Value
				};

				scope.SetExtraData("TenantContext", tenantContext);
			}

			return base.Create(context);
		}
	}
}