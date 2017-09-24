using Heroes.Api.Infrastructure;
using Heroes.Clients;
using Heroes.Contracts.Grains.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heroes.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureClusterClient();
			services.AddHeroesClients();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			IWarmUpClient warmUpClient)
		{
			warmUpClient.Initialize();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
