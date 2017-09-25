using GraphQl.AspNetCore;
using GraphQL.Types;
using Heroes.Api.GraphQLCore;
using Heroes.Api.GraphQLCore.Queries;
using Heroes.Api.Infrastructure;
using Heroes.Clients;
using Heroes.Contracts.Grains.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;

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
			services.AddHeroesAppGraphQL();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			IWarmUpClient warmUpClient,
			IClusterClient clusterClient,
			ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			warmUpClient.Initialize();

			app.SetGraphQLMiddleWare();
			//app.UseGraphQl(options =>
			//{
			//	options.GraphApiUrl = "/graphql";
			//	options.RootGraphType = new HeroesAppGraphQuery(clusterClient);
			//});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseGraphiQl();
			}

			app.UseMvc();
		}
	}
}